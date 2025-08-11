// Helpful website: https://react-typescript-cheatsheet.netlify.app/docs/basic/getting-started/context/

import type React from "react";
import { createContext, useEffect, useState } from "react";
import { produce } from "immer";
import type { TopicModel } from "../models";

type TopicContextType = {
    topics: TopicModel[];
    updateTopics: (fn: (draft: TopicModel[]) => void) => void;
};
export const TopicContext = createContext<TopicContextType>({ topics: [], updateTopics: () => () => {} });

type Props = {
    children: React.ReactNode;
};
export const TopicProvider = ({ children }: Props) => {
    const [topics, setTopics] = useState<TopicModel[]>([]);
    const [didUpdate, setDidUpdate] = useState(false);

    useEffect(() => {
        let storedTopics = localStorage.getItem("topics");
        if (storedTopics === null || storedTopics === "[]") {
            const defaultTopics = getTopics();
            setTopics(defaultTopics);
            localStorage.setItem("topics", JSON.stringify(defaultTopics));
            return;
        }

        try {
            const parsedTopics = JSON.parse(storedTopics) as TopicModel[];
            setTopics(parsedTopics);
        } catch {
            console.warn("Could not parse the topics form the local storage.");
            const defaultTopics = getTopics();
            setTopics(defaultTopics);
        }
    }, []);

    useEffect(() => {
        if (didUpdate)
            localStorage.setItem("topics", JSON.stringify(topics));
    }, [topics, didUpdate]);

    const updateTopics = (fn: (draft: TopicModel[]) => void) => {
        setTopics(prev => produce(prev, fn));
        setDidUpdate(true);
    };

    return (
        <TopicContext.Provider value={{ topics, updateTopics }}>
            {children}
        </TopicContext.Provider>
    );
};

function getTopics(): TopicModel[] {
        return [
        {
            id: 1,
            name: "Housework",
            statuses: [
                {
                    id: 1,
                    name: "Todo",
                    items: [
                        { id: 1, title: "Use vacuum cleaner", description: null, isDone: false },
                        { id: 2, title: "Dusting", description: null, isDone: false },
                        { id: 3, title: "Throw away trash", description: null, isDone: false },
                    ],
                },
                {
                    id: 2,
                    name: "Doing",
                    items: [
                        { id: 4, title: "Change bed sheets", description: null, isDone: false },
                    ],
                },
                {
                    id: 3,
                    name: "Done",
                    items: [
                        { id: 5, title: "Sort cloths", description: null, isDone: true },
                    ],
                },
                {
                    id: 4,
                    name: "Won't do",
                    items: [
                        { id: 6, title: "Mop the floor", description: null, isDone: false },
                    ],
                },
            ],
        }
    ];
}
