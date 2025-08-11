import { useContext, useState, type Dispatch, type MouseEvent, type SetStateAction, type TransitionEvent } from "react";
import styles from "./ProjectPage.module.scss";
import { TopicContext } from "../../stores/TopicProvider";
import { Pencil, Plus, Trash } from "lucide-react";
import { ItemForm, type ItemFormState } from "./ItemForm";
import type { ItemModel, StatusModel, TopicModel } from "../../models";

export default function ProjectPage() {
    const { topics: topics } = useContext(TopicContext);
    const [itemFormState, controlItemForm] = useState<ItemFormState>({ mode: "create", shouldOpen: false, topic: null, status: null, item: null });

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
    topic: TopicModel;
    controlItemForm: Dispatch<SetStateAction<ItemFormState>>;
}

function Topic({ topic, controlItemForm }: Props) {
    const addItem = (statusName: string, statusId: number) =>
        () => {
            controlItemForm({
                mode: "create",
                shouldOpen: true,
                topic: { id: topic.id, name: topic.name },
                status: { id: statusId, name: statusName },
                item: null,
            });
            const plusIcon = document.querySelector(`[data-plus-icon="${statusId}"]`);
            requestAnimationFrame(() => plusIcon?.scrollIntoView());
        };

    return (
        // TODO: figure out if it should be an article or something else.
        <article className={styles.topic} onClick={() => {}}>
            <h2 className={styles.topicName}>{topic.name}</h2>

            <div className={styles.statuses} data-statuses onTransitionEnd={() => {}}>
                {topic.statuses.map((status, key) => (
                    <section key={key} className={styles.status}>
                        {/* TODO: could add a count after the name. */}
                        <header className={styles.statusHeader}>
                            <h3 className={styles.statusName}>{status.name}</h3>
                            <div data-plus-icon={status.id} onClick={addItem(status.name, status.id)}>
                                <Plus size={18} className={styles.statusPlusIcon}/>
                            </div>
                        </header>

                        <ul key={key} className={styles.items}>
                            {status.items.map((item, key) => (
                                <li key={key}>
                                    <Item item={item} topic={topic} status={status} controlItemForm={controlItemForm} />
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
    item: ItemModel;
    topic: TopicModel;
    status: StatusModel;
    controlItemForm: Dispatch<SetStateAction<ItemFormState>>;
};

// TODO: make it so items kan be disabled, and make the text grey or have another visual indicator.
function Item({ item, topic, status, controlItemForm }: ItemProp) {
    const { updateTopics } = useContext(TopicContext);

    const edit = () => {
        controlItemForm({
            mode: "update",
            shouldOpen: true,
            topic,
            status,
            item,
        });
        const plusIcon = document.querySelector(`[data-plus-icon="${status.id}"]`);
        requestAnimationFrame(() => plusIcon?.scrollIntoView());
    };

    const check = () => {
        updateTopics(draft => {
            const topicTarget = draft.find(t => t.id === topic.id);
            const statusTarget = topicTarget?.statuses.find(s => s.id === status?.id);
            const itemTarget = statusTarget?.items.find(i => i.id == item.id);

            if (itemTarget)
                itemTarget.isDone = !itemTarget.isDone;
        });
    };

    const remove = () => {
        updateTopics(draft => {
            const topicTarget = draft.find(t => t.id === topic.id);
            const statusTarget = topicTarget?.statuses.find(s => s.id === status?.id);
            const itemIndex = statusTarget?.items.findIndex(i => i.id == item.id);

            if (itemIndex !== undefined && itemIndex !== -1)
                statusTarget?.items.splice(itemIndex, 1);
        });
    };

    return (
        <div className={styles.item}>
            <input type="checkbox" checked={item.isDone} className={styles.checkbox} onChange={check}/>

            <p className={styles.title}>{ item.title }</p>

            <div className={styles.controls}>
                <Pencil size={18} className={styles.controlButton} onClick={edit}/>
                <Trash size={18} className={styles.controlButton} onClick={remove}/>
            </div>
        </div>
    );
}
