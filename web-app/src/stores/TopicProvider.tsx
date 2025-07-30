// Helpful website: https://react-typescript-cheatsheet.netlify.app/docs/basic/getting-started/context/

import type React from "react";
import { createContext, useEffect, useState } from "react";
import { produce } from "immer";

export type TopicData = {
    id: number;
    name: string;
    statuses: Array<
        {
            id: number;
            name: string;
            items: Array<{
                id: number;
                title: string;
                // TODO: add 'done' boolean.
                // TODO: add nested items.
            }>;
        }
    >;
}

type TopicContextType = {
    topicData: TopicData[];
    updateTopics: (fn: (draft: TopicData[]) => void) => void;
};
export const TopicContext = createContext<TopicContextType>({ topicData: [], updateTopics: () => () => {} });

type Props = {
    children: React.ReactNode;
};
export const TopicProvider = ({ children }: Props) => {
    const [topicData, setTopicData] = useState<TopicData[]>([]);

    useEffect(() => {
        const topicData = getTopicData();
        setTopicData(topicData);
    }, []);

    const updateTopics = (fn: (draft: TopicData[]) => void) => {
        setTopicData(prev => produce(prev, fn));
    };

    return (
        <TopicContext.Provider value={{ topicData, updateTopics }}>
            {children}
        </TopicContext.Provider>
    );
};

function getTopicData(): Array<TopicData> {
        return [
        {
            id: 1,
            name: "Housework",
            statuses: [
                {
                    id: 1,
                    name: "Todo",
                    items: [
                        { id: 1, title: "Use vacuum cleaner" },
                        { id: 2, title: "Dusting" },
                        { id: 3, title: "Throw away trash" },
                    ],
                },
                {
                    id: 2,
                    name: "Doing",
                    items: [
                        { id: 4, title: "Change bed sheets" },
                    ],
                },
                {
                    id: 3,
                    name: "Done",
                    items: [
                        { id: 5, title: "Sort cloths" },
                    ],
                },
                {
                    id: 4,
                    name: "Won't do",
                    items: [
                        { id: 6, title: "Mop the floor" },
                    ],
                },
            ],
        }
    ];
}
