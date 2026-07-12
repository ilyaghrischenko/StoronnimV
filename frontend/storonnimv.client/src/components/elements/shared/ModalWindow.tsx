import {FC, useContext, useEffect} from "react";
import {GlobalContext} from "../../contexts/shared/GlobalContext";

const ModalWindow: FC = () => {
    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {showModal, OnHideModal, modalContent, modalTitle} = context;

    useEffect(() => {
    }, [modalContent, modalTitle]);

    return (
        <div className={showModal ? "modal active" : "modal"} onClick={OnHideModal}>
            <h1>{modalTitle}</h1>
            <div className= {showModal ? "modal__content active" : "modal__content"} onClick={e => e.stopPropagation()}>
                {modalContent}
            </div>
        </div>
    );
};

export {ModalWindow};
