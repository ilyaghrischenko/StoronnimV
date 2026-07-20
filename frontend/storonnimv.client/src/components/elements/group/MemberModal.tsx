import {FC, useContext, useEffect} from "react";
import {GroupContext} from "../../contexts/GroupContext.tsx";
import {Image, Button} from "react-bootstrap";
import {ModalLoading} from "../shared/ModalLoading.tsx";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {FaEdit, FaPlus, FaTrash} from "react-icons/fa";
import {MdDeleteForever} from "react-icons/md";
import {EditSocialModal} from "./forms/social/EditSocialModal.tsx";
import {DeleteSocialModal} from "./forms/social/DeleteSocialModal.tsx";
import {AddSocialModal} from "./forms/social/AddSocialModal.tsx";
import {DeleteMemberModal} from "./forms/member/DeleteMemberModal.tsx";
import {EditMemberModal} from "./forms/member/EditMemberModal.tsx";
import {NoData} from "../shared/NoData.tsx";

interface MemberModalProps {
    memberId: number;
}

const MemberModal: FC<MemberModalProps> = ({memberId}) => {
    const globalContext = useContext(GlobalContext)!;
    const groupContext = useContext(GroupContext)!;

    const {isAdmin, OnShowModal} = globalContext;
    const {fetchMemberInfo, memberFullInfo, memberStatus} = groupContext;

    useEffect(() => {
        void fetchMemberInfo(memberId);
    }, [fetchMemberInfo, memberId]);

    if (memberStatus === "loading") {
        return <ModalLoading/>;
    }

    if (memberStatus === "error") {
        return <NoData
            variant="error"
            message='Не вдалося завантажити дані учасника'
            actionLabel='Спробувати ще раз'
            onAction={() => void fetchMemberInfo(memberId)}
        />;
    }

    if (memberStatus === "empty") {
        return <NoData message='Дані про учасника відсутні'/>;
    }

    return (
        <div className="member-modal">
            <div className='member-modal__up-container'>
                <div className='member-modal__photo-container'>
                    <Image
                        alt={`Фото учасника ${memberFullInfo.fullName}`}
                        className="member-modal__photo"
                        src={memberFullInfo.photoUrl}
                    />
                </div>

                <div className="member-modal__info">
                    <h1 className="member-modal__info-title main-text">{memberFullInfo.fullName}</h1>
                    <h2 className="member-modal__info-role secondary-text">{memberFullInfo.role}</h2>

                    {isAdmin &&
                        <>
                            <Button
                                className='admin-button__edit'
                                onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
                                    event.preventDefault();
                                    OnShowModal(<EditMemberModal item={memberFullInfo}/>);
                                }}
                            >
                                <FaEdit/>
                            </Button>

                            <Button
                                className='admin-button__delete'
                                onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
                                    event.preventDefault();
                                    OnShowModal(<DeleteMemberModal item={memberFullInfo}/>);
                                }}
                            >
                                <FaTrash/>
                            </Button>
                        </>}
                </div>
            </div>

            <div className='member-modal__down-container'>
                <p className="member-modal__description secondary-text">{memberFullInfo.description}</p>

                <div className="member-modal__social-networks">
                    {isAdmin &&
                        <Button
                            className='admin-button__add--social'
                            onClick={() => OnShowModal(<AddSocialModal memberId={memberId}/>)}
                        >
                            <FaPlus/>
                        </Button>}

                    {memberFullInfo.socials.map((socialNetwork) => (
                        <div key={socialNetwork.id} className="member-modal__social-networks__item">
                            <p className="member-modal__social-networks__item-name secondary-text">
                                {socialNetwork.socialNetwork}
                            </p>
                            <a
                                className="member-modal__social-networks__item-link"
                                href={socialNetwork.url}
                                target="_blank"
                                rel="noopener noreferrer"
                            >
                                {socialNetwork.url}
                            </a>

                            {isAdmin &&
                                <Button
                                    className='admin-button__edit--social'
                                    onClick={() => OnShowModal(<EditSocialModal item={socialNetwork}/>)}
                                >
                                    <FaEdit/>
                                </Button>}

                            {isAdmin &&
                                <Button
                                    className='admin-button__delete--social'
                                    onClick={() => OnShowModal(<DeleteSocialModal itemId={socialNetwork.id}/>)}
                                >
                                    <MdDeleteForever/>
                                </Button>}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export {MemberModal};
