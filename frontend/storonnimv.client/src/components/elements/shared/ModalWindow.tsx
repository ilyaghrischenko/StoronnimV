import {FC, KeyboardEvent, useContext, useEffect, useRef} from "react";
import {Modal} from "react-bootstrap";
import {GlobalContext} from "../../contexts/shared/GlobalContext";

const ModalWindow: FC = () => {
    const context = useContext(GlobalContext);

    if (!context) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {showModal, OnHideModal, modalContent, modalTitle} = context;

    const closeButtonRef = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        if (showModal) {
            closeButtonRef.current?.focus();
        }
    }, [modalContent, showModal]);

    const handleKeyDown = (event: KeyboardEvent<HTMLElement>) => {
        if (event.key !== "Tab") {
            return;
        }

        const dialog = closeButtonRef.current?.closest<HTMLElement>('[role="dialog"]');
        if (!dialog) {
            return;
        }

        const focusableElements = Array.from(dialog.querySelectorAll<HTMLElement>(
            'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), video[controls], iframe, [tabindex]:not([tabindex="-1"])'
        )).filter(element =>
            !element.classList.contains("app-modal__focus-sentinel") &&
            element.getClientRects().length > 0
        );
        const firstElement = focusableElements[0];
        const lastElement = focusableElements.at(-1);

        if (event.shiftKey && document.activeElement === firstElement) {
            event.preventDefault();
            lastElement?.focus();
        } else if (!event.shiftKey && document.activeElement === lastElement) {
            event.preventDefault();
            firstElement?.focus();
        }
    };

    return (
        <Modal
            show={showModal}
            onHide={OnHideModal}
            bsPrefix="app-modal"
            dialogClassName="app-modal-dialog"
            contentClassName="app-modal-content"
            backdrop
            keyboard
            autoFocus
            enforceFocus
            restoreFocus
            scrollable
            aria-label={modalTitle || "Діалогове вікно"}
            onKeyDown={handleKeyDown}
        >
            <button
                ref={closeButtonRef}
                type="button"
                className="app-modal__close"
                aria-label="Закрити діалогове вікно"
                onClick={OnHideModal}
            >
                <span aria-hidden="true">×</span>
            </button>
            {modalTitle && <h1 className="app-modal__title">{modalTitle}</h1>}
            <Modal.Body bsPrefix="app-modal__body">
                {modalContent}
            </Modal.Body>
            <span
                className="app-modal__focus-sentinel"
                tabIndex={0}
                onFocus={() => closeButtonRef.current?.focus()}
            />
        </Modal>
    );
};

export {ModalWindow};
