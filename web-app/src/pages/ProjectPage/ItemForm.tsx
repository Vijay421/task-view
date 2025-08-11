import { useContext, useEffect, useRef, useState, type Dispatch, type FormEvent, type SetStateAction } from "react";
import styles from "./ItemForm.module.scss";
import { TopicContext } from "../../stores/TopicProvider";
import { X, CornerDownRight, ArrowRight } from "lucide-react";
import preventDefault from "../../scripts/FormPreventDefault";

export type ItemFormState = {
    mode: "create" | "update";
    shouldOpen: boolean;

    topic: { id: number, name: string } | null;
    status: { id: number, name: string } | null;
    item: { id: number, title: string, description: string | null } | null;
};

type FieldStatus = {
    title: null | "too long" | "empty" | true;
    msg: string | null;
};

type Props = {
    itemFormState: ItemFormState;
    controlItemForm: Dispatch<SetStateAction<ItemFormState>>;
};

// TODO: view details, edit items and item creation.
export function ItemForm({ itemFormState, controlItemForm }: Props) {
    const resetItemFormState = () => controlItemForm(old => ({ ...old, shouldOpen: false, topic: null, status: null, item: null }));

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [fieldStatus, setFieldStatus] = useState<FieldStatus>({ title: null, msg: null });
    const dialog = useRef<HTMLDialogElement>(null);
    const { updateTopics } = useContext(TopicContext);

    useEffect(() => {
        if (itemFormState.shouldOpen) {
            dialog?.current?.classList.add(`${styles.open}`);

            if (itemFormState.mode === "update") {
                setTitle(itemFormState.item?.title || "");
                setDescription(itemFormState.item?.description || "");
            }
        } else {
            dialog?.current?.classList.remove(`${styles.open}`);
        }
    }, [itemFormState]);

    const handleTitle = (e: FormEvent<HTMLInputElement>) => setTitle(e.currentTarget.value);
    const handleDescription = (e: FormEvent<HTMLInputElement>) => setDescription(e.currentTarget.value);

    const createItem = () => {
        updateTopics(draft => {
            const topic = draft.find(t => t.id === itemFormState.topic?.id);
            const status = topic?.statuses.find(s => s.id === itemFormState.status?.id);
            const id = -Math.floor(Math.random() * 999); // TODO: remove temporary ids generator.
            if (status)
                status.items.push({ id, title, description, isDone: false });
        });
    };

    const updateItem = () => {
        updateTopics(draft => {
            const topic = draft.find(t => t.id === itemFormState.topic?.id);
            const status = topic?.statuses.find(s => s.id === itemFormState.status?.id);
            const item =  status?.items.find(i => i.id === itemFormState.item?.id);
            if (item) {
                item.title = title;
                item.description = description;
            }
        });
    };

    const onSave = () => {
        const isValid = validateItem(title, description, setFieldStatus);
        if (!isValid) return;

        if (itemFormState.mode === "create")
            createItem();
        else
            updateItem();

        setTitle("");
        setDescription("");
        resetItemFormState();
    };

    return (
        // TODO: close the dialog when pressing: the back button, a close element or clicking outside the modal.
        // MDN: https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#browser_compatibility
        // <section ref={dialog} className={`${itemFormStyles.dialog} ${itemFormStyles.close}`}>
        <section ref={dialog} className={styles.dialog}>
            <header className={styles.header}>
                <h2 className={styles.headerTitle}>{ itemFormState.mode === "create" ? "Create" : "Change" } an item</h2>

                <div className={styles.headerDetail}>
                    <CornerDownRight className={styles.headerDetailArrow} size={16} />
                    <p className={styles.headerDetailText}>{itemFormState.topic?.name} </p>
                    <ArrowRight size={18} />
                    <p className={styles.headerDetailText}>{itemFormState.status?.name} </p>
                </div>

                <div className={styles.headerButton}>
                    <X size={18} onClick={resetItemFormState} />
                </div>
            </header>

            {/* TODO: create sub items as well. */}
            <form className={styles.form} onSubmit={preventDefault}>
                <div className={styles.inputGroup}>
                    <input type="text" placeholder="Title" onChange={handleTitle} value={title}/>
                    { fieldStatus.msg && <p className={styles.errorText}>{fieldStatus.msg}</p> }
                </div>

                {/* TODO: change to textarea. */}
                <input type="text" placeholder="Description" onChange={handleDescription} value={description}/>

                <button className={styles.saveButton} onClick={onSave}>Save</button>
            </form>
        </section>
    );
}

function validateItem(name: string, _description: string, setFieldStatus: Dispatch<SetStateAction<FieldStatus>>): boolean {
    switch (true) {
        case name.length === 0:
            setFieldStatus({ title: "empty", msg: "Title is required." });
            return false;

        case name.length > 50:
            setFieldStatus({ title: "too long", msg: "Must be 50 characters or fewer." });
            return false;

        default:
            setFieldStatus({ title: true, msg: null });
            return true;
    }
}
