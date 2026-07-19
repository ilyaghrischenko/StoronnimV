export const videoCategories = ["Performance", "Backstage", "Repetition"] as const;

export type VideoCategory = (typeof videoCategories)[number];
export type VideoType = VideoCategory | "Promotion";

export const isVideoCategory = (value: string | null): value is VideoCategory =>
    value !== null && videoCategories.some(category => category === value);

export interface IVideoModel {
    id: number;
    title: string;
    url: string;
    type: VideoType;
}
