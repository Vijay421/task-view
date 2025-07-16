import { type MouseEvent, type TransitionEvent } from "react";
import styles from "./ProjectPage.module.scss";

function ProjectPage() {
    const topics = [
        {
            name: "Housework",
            statuses: [
                {
                    name: "Todo",
                    items: [
                        { title: "Use vacuum cleaner" },
                        { title: "Dusting" },
                        { title: "Throw away trash" },
                    ],
                },
                {
                    name: "Doing",
                    items: [
                        { title: "Change bed sheets" },
                    ],
                },
                {
                    name: "Done",
                    items: [
                        { title: "Sort cloths" },
                    ],
                },
                {
                    name: "Won't do",
                    items: [
                        { title: "Mop the floor" },
                    ],
                },
            ],
        }
    ];

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
            <section className={styles.topics}>

                {topics.map((topic, key) => (
                    <article key={key} className={styles.topic} onClick={openStatuses}>
                        <h2 className={styles.topicName}>{topic.name}</h2>

                        <div className={styles.statuses} data-statuses onTransitionEnd={statusesTransitionEnd}>
                            {topic.statuses.map((status, key) => (
                                <section key={key} className={styles.status}>
                                    <h3 className={styles.statusName}>{status.name}</h3>

                                    <ul key={key} className={styles.items}>
                                    {status.items.map((item, key) => (
                                            <li key={key} className={styles.item}>{item.title}</li>
                                        ))}
                                    </ul>
                                </section>
                            ))}
                        </div>
                    </article>
                ))}

            </section>
        </main>
    );
}

export default ProjectPage;
