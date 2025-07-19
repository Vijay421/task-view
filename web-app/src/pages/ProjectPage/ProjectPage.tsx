import { useContext, type MouseEvent, type TransitionEvent } from "react";
import styles from "./ProjectPage.module.scss";
import { TopicContext } from "../../stores/TopicProvider";

function ProjectPage() {
    const topics = useContext(TopicContext);

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
        <main className="page">

            {/* TODO: might change the section to a list, which contains the topics */}
            <section className={styles.topics}>

                {topics.map((topic, key) => (
                    <Topic key={key} topic={topic} />
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
        </main>
    );
}

type Props = {
    topic: TopicProp;
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

function Topic({ topic }: Props) {
    return (
        // TODO: figure out if it should be an article of something else.
        <article className={styles.topic} onClick={() => {}}>
            <h2 className={styles.topicName}>{topic.name}</h2>

            <div className={styles.statuses} data-statuses onTransitionEnd={() => {}}>
                {topic.statuses.map((status, key) => (
                    <section key={key} className={styles.status}>
                        {/* TODO: maybe put a count after the name? */}
                        <h3 className={styles.statusName}>{status.name}</h3>

                        <ul key={key} className={styles.items}>
                            {status.items.map((item, key) => (
                                // TODO: make an item component.
                                // <li key={key} className={styles.item}>{item.title}</li>
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

export default ProjectPage;
