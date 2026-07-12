import React, {createContext, ReactNode, useContext, useState} from "react";
import {IGroupPageFullInfo} from "../../models/group/IGroupInfo";
import {GlobalContext} from "./shared/GlobalContext";
import {IMemberFullInfo} from "../../models/group/IMemberInfo.ts";

interface GroupContextType {
    fetchGroupInfo: () => Promise<void>;
    fullInfo: IGroupPageFullInfo;
    memberFullInfo: IMemberFullInfo;
    fetchMemberInfo: (memberId: number) => Promise<void>;
}

const GroupContext = createContext<GroupContextType | undefined>(undefined);

interface GroupContextProviderProps {
    children: ReactNode;
}

const GroupContextProvider: React.FC<GroupContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setPageLoading, setModalLoading, serverRoute} = globalContext;

    const [fullInfo, setFullInfo] = useState<IGroupPageFullInfo>({
        groupPage: {
            id: 0,
            photoUrl: "",
            description: ""
        },
        members: []
    });

    const fetchGroupInfo = async (): Promise<void> => {
        try {
            setPageLoading(true);
            const response = await sendRequest(`${serverRoute}/group`);

            const data: IGroupPageFullInfo = response.data;

            if (!data) {
                console.error("No data received from the API");
                return;
            }

            setFullInfo(data);
        } catch (error) {
            console.error("Error fetching news:", error);
            return;
        } finally {
            setPageLoading(false);
        }
    };

    const [memberFullInfo, setMemberFullInfo] = useState<IMemberFullInfo>({
        id: 0,
        photoUrl: "",
        fullName: "",
        description: "",
        role: "",
        socials: []
    });
    const fetchMemberInfo = async (memberId: number): Promise<void> => {
        try {
            setModalLoading(true);
            const response = await sendRequest(`${serverRoute}/group/member/${memberId}`);

            const data: IMemberFullInfo = response.data;

            if (!data) {
                console.error("No data received from the API");
                return;
            }
            setMemberFullInfo(data);
        } catch (error) {
            console.error("Error fetching news:", error);
            return;
        } finally {
            setModalLoading(false);
        }
    };

    const value: GroupContextType = {
        memberFullInfo,
        fetchMemberInfo,
        fetchGroupInfo,
        fullInfo,
    };

    return <GroupContext.Provider value={value}>{children}</GroupContext.Provider>;
};

export {GroupContext, GroupContextProvider};
