import {FC, useContext, useEffect, useState} from "react";
import {Button, Image} from "react-bootstrap";

import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {FaEdit, FaPlus, FaTrash} from "react-icons/fa";
import {IGroupSocial} from "../../../models/groupSocials/IGroupSocial.ts";
import {DeleteGroupSocialModal} from "../group/forms/groupSocial/DeleteGroupSocialModal.tsx";
import {EditGroupSocialModal} from "../group/forms/groupSocial/EditGroupSocialModal.tsx";
import {AddGroupSocialModal} from "../group/forms/groupSocial/AddGroupSocialModal.tsx";

const Footer: FC = () => {
    const globalContext = useContext(GlobalContext)!;

    const {isAdmin, OnShowModal, sendRequest, setPageLoading, serverRoute} = globalContext;

    const [groupSocials, setGroupSocials] = useState<IGroupSocial[]>([]);

    const fetchGroupSocials = async () => {
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
    };

    useEffect(() => {
        fetchGroupSocials();
    }, []);

    const [hoveredIndex, setHoveredIndex] = useState<number | null>(null);

    return (
        <div className='footer-container'>
            {isAdmin &&
                <Button
                    className="footer-container__add-button"
                    onClick={() => OnShowModal(<AddGroupSocialModal />)}>
                    <FaPlus/>
                </Button>}

            {groupSocials.map((social, index) => (
                <div
                    key={index}
                    className='footer-container__item'
                    onMouseEnter={() => setHoveredIndex(index)}
                    onMouseLeave={() => setHoveredIndex(null)}
                >
                    <a href={social.linkUrl} target="_blank" rel="noopener noreferrer" className='footer-container__link'>
                        <Image src={social.photoUrl} className='footer-container__link-photo'/>
                    </a>

                    {isAdmin && hoveredIndex === index && (
                        <div className='group-socials-admin-buttons-container'>
                            <Button
                                className="group-socials-admin-buttons-container__edit"
                                onClick={() => OnShowModal(<EditGroupSocialModal item={social} />)}
                            >
                                <FaEdit/>
                            </Button>
                            <Button
                                className="group-socials-admin-buttons-container__delete"
                                onClick={() => OnShowModal(<DeleteGroupSocialModal itemId={social.id} />)}
                            >
                                <FaTrash/>
                            </Button>
                        </div>
                    )}
                </div>
            ))}
        </div>
    );
};

export {Footer};