import {FC, useContext, useEffect} from "react";
import {Button, Container} from "react-bootstrap";
import {Description} from "./groupPageComponents/Description";
import {ShortMembers} from "./groupPageComponents/ShortMembers";
import {GroupContext} from "../../contexts/GroupContext";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {FaEdit} from "react-icons/fa";
import {EditGroupModal} from "./forms/group/EditGroupModal.tsx";
import {NoData} from "../shared/NoData.tsx";

const GroupDescription: FC = () => {
    const groupContext = useContext(GroupContext)!;
    const globalContext = useContext(GlobalContext)!;

    const {isAdmin, OnShowModal} = globalContext;
    const {fetchGroupInfo, fullInfo, groupStatus} = groupContext;

    useEffect(() => {
        void fetchGroupInfo();
    }, [fetchGroupInfo]);

    if (groupStatus === "loading") {
        return (
            <Container className='group-description-container'>
                <PreloaderTile className='preloader-tile__container-group-page'/>
            </Container>
        );
    }

    if (groupStatus === "error") {
        return <NoData
            variant="error"
            message='Не вдалося завантажити дані про групу'
            actionLabel='Спробувати ще раз'
            onAction={() => void fetchGroupInfo()}
        />;
    }

    if (groupStatus === "empty") {
        return <NoData message='Дані про групу відсутні'/>;
    }

    const backgroundStyle = fullInfo.groupPage.photoUrl
        ? {
            backgroundImage: `linear-gradient(to bottom, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.5) 40%, rgba(0, 0, 0, 0.8) 100%), url(${fullInfo.groupPage.photoUrl})`
        }
        : undefined;

    return (
        <Container className='group-description-container' style={backgroundStyle}>
            {isAdmin && <Button
                className="admin-button__edit"
                onClick={() => OnShowModal(<EditGroupModal fullInfo={fullInfo}/>)}
            >
                <FaEdit/>
            </Button>}

            {fullInfo.groupPage.description
                ? <Description groupInfo={fullInfo.groupPage}/>
                : <NoData message='Опис групи відсутній'/>}
            {fullInfo.members.length > 0
                ? <ShortMembers members={fullInfo.members}/>
                : <NoData message='Дані про учасників відсутні'/>}
        </Container>
    );
};

export {GroupDescription};
