import React, {createContext, ReactNode, useCallback, useContext, useState} from "react";
import {IGroupPageFullInfo} from "../../models/group/IGroupInfo";
import {GlobalContext} from "./shared/GlobalContext";
import {IMemberFullInfo} from "../../models/group/IMemberInfo.ts";

interface GroupContextType {
    fetchGroupInfo: () => Promise<void>;
    fullInfo: IGroupPageFullInfo;
    groupStatus: RequestStatus;
    memberFullInfo: IMemberFullInfo;
    memberStatus: RequestStatus;
    fetchMemberInfo: (memberId: number) => Promise<void>;
}

type RequestStatus = "loading" | "success" | "empty" | "error";

const emptyGroupInfo: IGroupPageFullInfo = {
    groupPage: {id: 0, photoUrl: "", description: ""},
    members: []
};

const emptyMemberInfo: IMemberFullInfo = {
    id: 0,
    photoUrl: "",
    fullName: "",
    description: "",
    role: "",
    socials: []
};

const GroupContext = createContext<GroupContextType | undefined>(undefined);

interface GroupContextProviderProps {
    children: ReactNode;
}

const GroupContextProvider: React.FC<GroupContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setPageLoading, setModalLoading, serverRoute} = globalContext;

    const [fullInfo, setFullInfo] = useState<IGroupPageFullInfo>(emptyGroupInfo);
    const [groupStatus, setGroupStatus] = useState<RequestStatus>("loading");

    const fetchGroupInfo = useCallback(async (): Promise<void> => {
        setPageLoading(true);
        setFullInfo(emptyGroupInfo);
        setGroupStatus("loading");
        try {
            const response = await sendRequest(`${serverRoute}/group`);
            if (response.status !== 200) {
                throw new Error(`Group request failed with status ${response.status}`);
            }

            const data = response.data as Partial<IGroupPageFullInfo> | null;
            if (
                !data?.groupPage ||
                typeof data.groupPage.id !== "number" ||
                typeof data.groupPage.photoUrl !== "string" ||
                typeof data.groupPage.description !== "string" ||
                !Array.isArray(data.members)
            ) {
                throw new Error("Group response is invalid");
            }

            const nextInfo: IGroupPageFullInfo = {
                groupPage: data.groupPage,
                members: data.members
            };
            setFullInfo(nextInfo);
            const isEmpty = !nextInfo.groupPage.photoUrl &&
                !nextInfo.groupPage.description &&
                nextInfo.members.length === 0;
            setGroupStatus(isEmpty ? "empty" : "success");
        } catch (error) {
            setFullInfo(emptyGroupInfo);
            setGroupStatus("error");
            console.error("Error fetching group:", error);
        } finally {
            setPageLoading(false);
        }
    }, [sendRequest, serverRoute, setPageLoading]);

    const [memberFullInfo, setMemberFullInfo] = useState<IMemberFullInfo>(emptyMemberInfo);
    const [memberStatus, setMemberStatus] = useState<RequestStatus>("loading");
    const fetchMemberInfo = useCallback(async (memberId: number): Promise<void> => {
        setModalLoading(true);
        setMemberFullInfo(emptyMemberInfo);
        setMemberStatus("loading");
        try {
            const response = await sendRequest(`${serverRoute}/group/member/${memberId}`);
            if (response.status === 404 || response.data == null) {
                setMemberStatus("empty");
                return;
            }
            if (response.status !== 200) {
                throw new Error(`Member request failed with status ${response.status}`);
            }

            const data = response.data as Partial<IMemberFullInfo>;
            if (
                typeof data.id !== "number" ||
                typeof data.photoUrl !== "string" ||
                typeof data.fullName !== "string" ||
                typeof data.description !== "string" ||
                typeof data.role !== "string" ||
                !Array.isArray(data.socials)
            ) {
                throw new Error("Member response is invalid");
            }

            setMemberFullInfo(data as IMemberFullInfo);
            setMemberStatus("success");
        } catch (error) {
            setMemberFullInfo(emptyMemberInfo);
            setMemberStatus("error");
            console.error("Error fetching member:", error);
        } finally {
            setModalLoading(false);
        }
    }, [sendRequest, serverRoute, setModalLoading]);

    const value: GroupContextType = {
        memberFullInfo,
        memberStatus,
        fetchMemberInfo,
        fetchGroupInfo,
        fullInfo,
        groupStatus,
    };

    return <GroupContext.Provider value={value}>{children}</GroupContext.Provider>;
};

export {GroupContext, GroupContextProvider};
