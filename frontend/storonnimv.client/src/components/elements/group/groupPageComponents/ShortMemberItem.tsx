import {Container} from "react-bootstrap";
import {IMemberShort} from "../../../../models/group/IGroupInfo";
import {FC} from "react";

interface IMemberItemProps {
    member: IMemberShort;
    onClick?: () => void;
}

const ShortMemberItem: FC<IMemberItemProps> = ({member, onClick}) => {
    return (
        <div className="short-member-item-container" onClick={onClick}>
            <Container
                className='photo-container'
                style={{
                    backgroundImage: `linear-gradient(to bottom, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.5) 70%, rgba(0, 0, 0, 0.8) 100%), url(${member.photoUrl})`
                }}
            >
                <p className='photo-container__full-name main-text big-shadow'>{member.fullName}</p>
                <p className='photo-container__role secondary-text small-shadow'>{member.role}</p>
            </Container>
        </div>
    );
};

export {ShortMemberItem};