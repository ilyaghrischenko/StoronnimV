import {FC, useEffect, useRef} from "react";
import {Container} from "react-bootstrap";
import {IGroupInfo} from "../../../../models/group/IGroupInfo";

interface IDescriptionProps {
    groupInfo: IGroupInfo;
}

const Description: FC<IDescriptionProps> = ({groupInfo}) => {
    const scrollRef = useRef<HTMLDivElement>(null);
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            const scrollElement = scrollRef.current;
            const containerElement = containerRef.current;

            if (scrollElement && containerElement) {
                const scrollHeight = scrollElement.scrollHeight;
                const containerHeight = containerElement.clientHeight;
                const distance = scrollHeight - containerHeight;

                if (distance <= 0) {
                    return;
                }

                const speed = 15; // пикселей в секунду вниз
                const downDuration = distance / speed;
                const upDuration = downDuration / 8; // вверх быстрее
                const totalDuration = downDuration + upDuration;

                scrollElement.animate([
                    { transform: 'translateY(0)', offset: 0 },
                    { transform: `translateY(-${distance}px)`, offset: downDuration / totalDuration },
                    { transform: 'translateY(0)', offset: 1 }
                ], {
                    duration: totalDuration * 1000,
                    iterations: Infinity,
                    easing: 'linear'
                });
            }
        }, 3000);

        return () => clearTimeout(timeoutId);
    }, []);

    return (
        <Container className='description-container'>
            <h1 className='description-container__group-name main-text big-shadow'>СТОРОННІМ В</h1>
            <div className='description-div' ref={containerRef}>
                <div className='scrolling-text' ref={scrollRef}>
                    <p className='description-div__description secondary-text small-shadow'>
                        {groupInfo.description}
                    </p>
                </div>
            </div>
        </Container>
    );
}

export {Description};
