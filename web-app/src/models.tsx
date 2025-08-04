export type TopicModel = {
    id: number;
    name: string;
    statuses: StatusModel[];
};

export type StatusModel = {
    id: number;
    name: string;
    items: ItemModel[];
};

export type ItemModel = {
    id: number;
    title: string;
    description: string | null;
    isDone: boolean;
};
