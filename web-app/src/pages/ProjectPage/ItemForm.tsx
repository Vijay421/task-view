import { useContext, useEffect, useRef, useState, type Dispatch, type FormEvent, type SetStateAction } from "react";
import styles from "./ItemForm.module.scss";
import { TopicContext } from "../../stores/TopicProvider";
import { X, ChevronRight } from "lucide-react";
import preventDefault from "../../scripts/FormPreventDefault";

export type ItemFormState = {
    isOpen: boolean;
    topicId: number | null;
    statusId: number | null;
    topicName: string | null;
    statusName: string | null;
};

type FieldStatus = {
    title: null | "too long" | "empty" | true;
};

type Props = {
    itemFormState: ItemFormState;
    controlItemForm: Dispatch<SetStateAction<ItemFormState>>;
};

// TODO: could rename to ItemModel and use is to view details, edit items and item creation.
export function ItemForm({ itemFormState, controlItemForm }: Props) {
    const resetItemFormState = () => controlItemForm({ isOpen: false, topicId: null, statusId: null, topicName: null, statusName: null });

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [fieldStatus, setFieldStatus] = useState<FieldStatus>({ title: null });
    const dialog = useRef<HTMLDialogElement>(null);
    const { updateTopics } = useContext(TopicContext);

    useEffect(() => {
        if (itemFormState.isOpen) {
            dialog?.current?.classList.add(`${styles.open}`);
        } else {
            dialog?.current?.classList.remove(`${styles.open}`);
        }
    }, [itemFormState]);

    const handleTitle = (e: FormEvent<HTMLInputElement>) => setTitle(e.currentTarget.value);
    const handleDescription = (e: FormEvent<HTMLInputElement>) => setDescription(e.currentTarget.value);
    const onSave = () => {
        const isValid = validateItem(title, description, setFieldStatus);
        if (!isValid) return;

        updateTopics(draft => {
            const topic = draft.find(t => t.id === itemFormState.topicId);
            const status = topic?.statuses.find(s => s.id === itemFormState.statusId);
            if (status)
                status.items.push({ id: 123, title });
        });

        resetItemFormState();
    };

    const titleText = getTitleText(fieldStatus);

    return (
        // TODO: close the dialog when pressing: the back button, a close element or clicking outside the modal.
        // MDN: https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#browser_compatibility
        // <section ref={dialog} className={`${itemFormStyles.dialog} ${itemFormStyles.close}`}>
        <section ref={dialog} className={styles.dialog}>
            <header className={styles.header}>
                <h2 className={styles.headerTitle}>Create an item</h2>

                <div className={styles.headerDetail}>
                    <p className={styles.headerDetailText}>{itemFormState.topicName} </p>
                    <ChevronRight size={18} />
                    <p className={styles.headerDetailText}>{itemFormState.statusName} </p>
                </div>

                <div className={styles.headerButton}>
                    <X size={18} className={styles.icon} onClick={resetItemFormState} />
                </div>
            </header>

            {/* TODO: create sub items as well. */}
            <form className={styles.form} onSubmit={preventDefault}>
                <div className={styles.inputGroup}>
                    <input type="text" placeholder="Title" onChange={handleTitle}/>
                    { titleText && <p className={styles.errorText}>{titleText}</p> }
                </div>
                <input type="text" placeholder="Description" onChange={handleDescription}/>

                <button className={styles.saveButton} onClick={onSave}>Save</button>
            </form>
        </section>
    );
}

function validateItem(name: string, _description: string, setFieldStatus: Dispatch<SetStateAction<FieldStatus>>): boolean {
    switch (true) {
        case name.length === 0:
            setFieldStatus({ title: "empty" });
            return false;

        case name.length > 50:
            setFieldStatus({ title: "too long" });
            return false;

        default:
            setFieldStatus({ title: true });
            return true;
    }
}

function getTitleText(fieldStatus: FieldStatus): string {
    switch(true) {
        case fieldStatus.title === "empty":
            return "Title is required.";

        case fieldStatus.title === "too long":
            return "Must be 50 characters or fewer.";

        case fieldStatus.title === true:
        case fieldStatus.title === null:
        default:
            return "";
    }
}
