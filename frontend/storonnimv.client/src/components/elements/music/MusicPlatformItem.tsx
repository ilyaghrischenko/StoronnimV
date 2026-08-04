import {FC, useContext} from "react";
import {Button, ListGroupItem} from "react-bootstrap";
import {IMusicPlatformItem} from "../../../models/music/IMusicPlatformItem";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {EditMusicPlatformModal} from "./forms/EditMusicPlatformModal.tsx";
import {FaEdit, FaTrash} from "react-icons/fa";
import {DeleteMusicPlatformModal} from "./forms/DeleteMusicPlatformModal.tsx";
import {getSafeExternalUrl} from "../../../utils/externalUrl.ts";

interface MusicPlatformItemProps {
    item: IMusicPlatformItem;
}

const MusicPlatformItem: FC<MusicPlatformItemProps> = ({item}) => {
    const globalContext = useContext(GlobalContext)!;

    const {isAdmin, OnShowModal} = globalContext;
    const safePlatformUrl = getSafeExternalUrl(item.platformUrl);

    return (
        <ListGroupItem
            as="li"
            className='music-platform-item'
            style={{backgroundImage: `url(${item.bgImageUrl})`}}
        >
            {safePlatformUrl ? <a
                    className="music-platform-item__link"
                    href={safePlatformUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    aria-label={`Відкрити музичну платформу ${item.id}`}
                /> : <span
                    role="img"
                    className="music-platform-item__link music-platform-item__link--disabled"
                    aria-label={`Музична платформа ${item.id}: посилання недоступне`}
                />}
            {isAdmin &&
                <div className="music-platform-item__admin-actions admin-controls">
                    <Button
                        className='admin-control'
                        type="button"
                        aria-label={`Редагувати музичну платформу ${item.id}`}
                        onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
                            e.preventDefault();

                            OnShowModal(<EditMusicPlatformModal item={item}/>)
                        }}>
                        <FaEdit/>
                    </Button>

                    <Button
                        className='admin-control'
                        type="button"
                        aria-label={`Видалити музичну платформу ${item.id}`}
                        onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
                        e.preventDefault();

                        OnShowModal(<DeleteMusicPlatformModal item={item}/>)
                    }}>
                        <FaTrash/>
                    </Button>
                </div>}
        </ListGroupItem>
    );
};

export {MusicPlatformItem};
