import {IMemberShort} from "../../../../models/group/IGroupInfo";
import {FC, useContext} from "react";
import {ShortMemberItem} from "./ShortMemberItem";
import {GlobalContext} from "../../../contexts/shared/GlobalContext";
import {AddMemberModal} from "../forms/member/AddMemberModal.tsx";
import {Button} from "react-bootstrap";
import {FaPlus} from "react-icons/fa";
import {Swiper, SwiperSlide} from "swiper/react";
import {Autoplay, Navigation} from "swiper/modules";
import {GroupContextProvider} from "../../../contexts/GroupContext.tsx";
import {MemberModal} from "../MemberModal.tsx";

interface IShortMembersProps {
    members: IMemberShort[];
}

const ShortMembers: FC<IShortMembersProps> = ({members}) => {
    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {OnShowModal, isAdmin} = context;

    return (
        <div className='short-members-container'>
            {isAdmin &&
                <Button
                    className='admin-button__add'
                    onClick={() => OnShowModal(<AddMemberModal/>)}>
                    <FaPlus/>
                </Button>}

            <div className="swiper">
                <Swiper
                    key={members.length}
                    modules={[Navigation, Autoplay]}
                    slidesPerView={3}
                    spaceBetween={20}
                    navigation
                    autoplay={{delay: 3000, disableOnInteraction: false}}
                    loop
                    speed={1800}
                >
                    {members.map((member, index) => (
                        <SwiperSlide key={index}>
                            <ShortMemberItem
                                key={index}
                                member={member}
                                onClick={() => OnShowModal(
                                    <GroupContextProvider>
                                        <MemberModal memberId={member.id} />
                                    </GroupContextProvider>
                                )}
                            />
                        </SwiperSlide>
                    ))}
                </Swiper>
            </div>
        </div>
    );
};

export {ShortMembers};
