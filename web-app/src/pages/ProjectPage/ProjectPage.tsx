import { useContext, useEffect, useRef, useState, type Dispatch, type FormEvent, type MouseEvent, type SetStateAction, type TransitionEvent } from "react";
import styles from "./ProjectPage.module.scss";
import itemFormStyles from "./ItemForm.module.scss";
import { TopicContext } from "../../stores/TopicProvider";
import { Plus, X, ChevronRight } from "lucide-react";
import preventDefault from "../../scripts/FormPreventDefault";

type ItemFormState = {
  isOpen: boolean;
  topicId: number | null;
  statusId: number | null;
  topicName: string | null;
  statusName: string | null;
};
function ProjectPage() {
    const { topicData: topics } = useContext(TopicContext);
    const [itemFormState, controlItemForm] = useState<ItemFormState>({ isOpen: false, topicId: null, statusId: null, topicName: null, statusName: null });

    const openStatuses = (e: MouseEvent<HTMLDivElement>) => {
        const statuses = e.currentTarget.querySelector("[data-statuses]") as HTMLDivElement;
        if (statuses === null)
            return;

        const height = statuses.scrollHeight;

        console.log("click", statuses.style.maxHeight);

        statuses.style.maxHeight = "0";
        requestAnimationFrame(() => {
            statuses.style.maxHeight = `${height}px`;
        });

        // if (statuses.style.maxHeight === "0") {
        //     statuses.style.maxHeight = "0";
        //     requestAnimationFrame(() => {
        //         statuses.style.maxHeight = `${height}px`;
        //     });
        // } else {
        //     statuses.style.maxHeight = `${height}px`;
        //     requestAnimationFrame(() => {
        //         statuses.style.maxHeight = "0";
        //     });
        // }

        // statuses.classList.toggle(styles.statusesShow);
    };

    const statusesTransitionEnd = (e: TransitionEvent<HTMLDivElement>) => {
        const statuses = e.currentTarget;
        statuses.style.maxHeight = "fit-content";
    };

    return (
        <main className={`page ${styles.page}`}>

            {/* TODO: might change the section to a list, which contains the topics */}
            <section className={styles.topics}>

                {topics.map((topic, key) => (
                    <Topic key={key} controlItemForm={controlItemForm} topic={topic} />
                    // <article key={key} className={styles.topic} onClick={openStatuses}>
                    //     <h2 className={styles.topicName}>{topic.name}</h2>

                    //     <div className={styles.statuses} data-statuses onTransitionEnd={statusesTransitionEnd}>
                    //         {topic.statuses.map((status, key) => (
                    //             <section key={key} className={styles.status}>
                    //                 <h3 className={styles.statusName}>{status.name}</h3>

                    //                 <ul key={key} className={styles.items}>
                    //                 {status.items.map((item, key) => (
                    //                     <li key={key} className={styles.item}>{item.title}</li>
                    //                 ))}
                    //                 </ul>
                    //             </section>
                    //         ))}
                    //     </div>
                    // </article>
                ))}

            </section>
            <ItemForm itemFormState={itemFormState} controlItemForm={controlItemForm} />
        </main>
    );
}

type Props = {
    topic: TopicProp;
    controlItemForm: Dispatch<SetStateAction<ItemFormState>>;
}
type TopicProp = {
    id: number;
    name: string;
    statuses: Array<
        {
            id: number;
            name: string;
            items: Array<{ id: number, title: string }>;
        }
    >;
}

function Topic({ topic, controlItemForm }: Props) {
    const plusIcon = useRef<HTMLDivElement>(null);

    const addItem = (topicName: string, statusName: string, statusId: number) =>
        () => {
            controlItemForm({ isOpen: true, topicId: topic.id, statusId, topicName, statusName });
            requestAnimationFrame(() => plusIcon.current?.scrollIntoView());
        };

    return (
        // TODO: figure out if it should be an article or something else.
        <article className={styles.topic} onClick={() => {}}>
            <h2 className={styles.topicName}>{topic.name}</h2>

            <div className={styles.statuses} data-statuses onTransitionEnd={() => {}}>
                {topic.statuses.map((status, key) => (
                    <section key={key} className={styles.status}>
                        {/* TODO: maybe put a count after the name? */}
                        <header className={styles.statusHeader}>
                            <h3 className={styles.statusName}>{status.name}</h3>
                            <div ref={plusIcon} onClick={addItem(topic.name, status.name, status.id)}>
                                <Plus size={18} className={styles.statusPlusIcon}/>
                            </div>
                        </header>

                        <ul key={key} className={styles.items}>
                            {status.items.map((item, key) => (
                                <li key={key}>
                                    <Item title={item.title} />
                                </li>
                            ))}
                        </ul>
                    </section>
                ))}
            </div>
        </article>
    );
}

type ItemProp = {
    title: string;
};
function Item({ title }: ItemProp) {
    return (
        <div className={styles.item}>
            <input type="checkbox" className={styles.checkbox}/>
            { title }
        </div>
    );
}

type ItemFormProp = {
    itemFormState: ItemFormState;
    controlItemForm: Dispatch<SetStateAction<ItemFormState>>;
};
type FieldStatus = {
    title: null | "too long" | "empty" | true;
};

// TODO: could rename to ItemModel and use is to view details, edit items and item creation.
function ItemForm({ itemFormState, controlItemForm }: ItemFormProp) {
    const resetItemFormState = () => controlItemForm({ isOpen: false, topicId: null, statusId: null, topicName: null, statusName: null });

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [fieldStatus, setFieldStatus] = useState<FieldStatus>({ title: null });
    const dialog = useRef<HTMLDialogElement>(null);
    const { updateTopics } = useContext(TopicContext);

    useEffect(() => {
        if (itemFormState.isOpen) {
            dialog?.current?.classList.add(`${itemFormStyles.open}`);
        } else {
            dialog?.current?.classList.remove(`${itemFormStyles.open}`);
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

    return (
        // TODO: close the dialog when pressing: the back button, a close element or clicking outside the modal.
        // MDN: https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#browser_compatibility
        // <section ref={dialog} className={`${itemFormStyles.dialog} ${itemFormStyles.close}`}>
        <section ref={dialog} className={`${itemFormStyles.dialog}`}>
            <header className={itemFormStyles.header}>
                <h2 className={itemFormStyles.headerTitle}>Create an item</h2>

                <div className={itemFormStyles.headerDetail}>
                    <p className={itemFormStyles.headerDetailText}>{itemFormState.topicName} </p>
                    <ChevronRight size={18} />
                    <p className={itemFormStyles.headerDetailText}>{itemFormState.statusName} </p>
                </div>

                <div className={itemFormStyles.headerButton}>
                    <X size={18} className={itemFormStyles.icon} onClick={resetItemFormState} />
                </div>
            </header>

            {/* TODO: create sub items as well. */}
            <form className={itemFormStyles.form} onSubmit={preventDefault}>
                <input type="text" placeholder="Title" onChange={handleTitle}/>
                <input type="text" placeholder="Description" onChange={handleDescription}/>

                <button className={itemFormStyles.saveButton} onClick={onSave}>Save</button>
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
            return true;
    }
}

export default ProjectPage;
