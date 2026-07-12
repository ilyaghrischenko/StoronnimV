
export interface IMember {
    id: number;
    photoUrl: string;
    fullName: string;
    description: string;
    role: string;
}

export interface ISocialNetwork {
    id: number;
    socialNetwork: string;
    url: string;
}

export interface IMemberFullInfo {
    id: number;
    photoUrl: string;
    fullName: string;
    description: string;
    role: string;
    socials: ISocialNetwork[];
}