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
import {NoData} from "../../shared/NoData.tsx";

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
                <div className="short-members-container__admin-actions admin-controls">
                    <Button
                        className='admin-control'
                        type="button"
                        aria-label="Додати учасника групи"
                        onClick={() => OnShowModal(<AddMemberModal/>)}>
                        <FaPlus/>
                    </Button>
                </div>}

            {members.length > 0 ? <Swiper
                key={members.length}
                modules={[Navigation, Autoplay]}
                slidesPerView={1}
                spaceBetween={12}
                breakpoints={{
                    640: {slidesPerView: 2, spaceBetween: 16},
                    1024: {slidesPerView: 3, spaceBetween: 20},
                }}
                navigation
                autoplay={{delay: 3000, disableOnInteraction: false}}
                loop={members.length > 3}
                speed={1800}
            >
                {members.map((member) => (
                    <SwiperSlide key={member.id}>
                        <ShortMemberItem
                            member={member}
                            onClick={() => OnShowModal(
                                <GroupContextProvider>
                                    <MemberModal memberId={member.id} />
                                </GroupContextProvider>
                            )}
                        />
                    </SwiperSlide>
                ))}
            </Swiper> : <NoData message='Дані про учасників відсутні'/>}
        </div>
    );
};

export {ShortMembers};
