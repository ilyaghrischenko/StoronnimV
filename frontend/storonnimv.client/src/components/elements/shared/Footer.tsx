import {FC, useContext, useEffect, useState} from "react";
import {Button, Image} from "react-bootstrap";
import {useCallback} from "react";

import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {FaEdit, FaPlus, FaTrash} from "react-icons/fa";
import {IGroupSocial} from "../../../models/groupSocials/IGroupSocial.ts";
import {DeleteGroupSocialModal} from "../group/forms/groupSocial/DeleteGroupSocialModal.tsx";
import {EditGroupSocialModal} from "../group/forms/groupSocial/EditGroupSocialModal.tsx";
import {AddGroupSocialModal} from "../group/forms/groupSocial/AddGroupSocialModal.tsx";
import {getSafeExternalUrl} from "../../../utils/externalUrl.ts";

const Footer: FC = () => {
    const globalContext = useContext(GlobalContext)!;

    const {isAdmin, OnShowModal, sendRequest, setPageLoading, serverRoute} = globalContext;

    const [groupSocials, setGroupSocials] = useState<IGroupSocial[]>([]);

    const fetchGroupSocials = useCallback(async () => {
        try {
            setPageLoading(true);

            const response = await sendRequest(`${serverRoute}/group-socials`);

            const data: IGroupSocial[] = response.data;
            setGroupSocials(data);
        } catch (error) {
            console.error('Error while fetching group socials', error);
        }
        finally {
            setPageLoading(false);
        }
    }, [sendRequest, serverRoute, setPageLoading]);

    useEffect(() => {
        void fetchGroupSocials();
    }, [fetchGroupSocials]);

    return (
        <div className='footer-container'>
            {isAdmin &&
                <Button
                    className="footer-container__add-button"
                    type="button"
                    aria-label="Додати посилання соціальної мережі"
                    onClick={() => OnShowModal(<AddGroupSocialModal />)}>
                    <FaPlus/>
                </Button>}

            {groupSocials.map((social) => {
                const safeLinkUrl = getSafeExternalUrl(social.linkUrl);

                return (
                <div
                    key={social.id}
                    className='footer-container__item'
                >
                    {safeLinkUrl ?
                        <a
                            href={safeLinkUrl}
                            target="_blank"
                            rel="noopener noreferrer"
                            aria-label={social.name}
                            className='footer-container__link'
                        >
                            <Image src={social.photoUrl} alt="" aria-hidden="true" className='footer-container__link-photo'/>
                        </a> :
                        <span
                            role="img"
                            aria-label={`${social.name}: посилання недоступне`}
                            className='footer-container__link footer-container__link--disabled'
                        >
                            <Image src={social.photoUrl} alt="" aria-hidden="true" className='footer-container__link-photo'/>
                        </span>}

                    {isAdmin && (
                        <div className='group-socials-admin-buttons-container'>
                            <Button
                                className="group-socials-admin-buttons-container__edit admin-control"
                                type="button"
                                aria-label={`Редагувати посилання соціальної мережі ${social.id}`}
                                onClick={() => OnShowModal(<EditGroupSocialModal item={social} />)}
                            >
                                <FaEdit/>
                            </Button>
                            <Button
                                className="group-socials-admin-buttons-container__delete admin-control"
                                type="button"
                                aria-label={`Видалити посилання соціальної мережі ${social.id}`}
                                onClick={() => OnShowModal(<DeleteGroupSocialModal itemId={social.id} />)}
                            >
                                <FaTrash/>
                            </Button>
                        </div>
                    )}
                </div>
                );
            })}
        </div>
    );
};

export {Footer};
