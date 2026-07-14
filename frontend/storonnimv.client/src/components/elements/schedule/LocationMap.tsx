import {FC} from "react";

interface ILocationMapProps {
    address: string;
}

const LocationMap: FC<ILocationMapProps> = ({address}) => {
    const encodedAddress = encodeURIComponent(address);
    const mapUrl = `https://www.google.com/maps?q=${encodedAddress}&output=embed`;

    return (
        <div className='location-map-container'>
            <iframe
                className='location-map-container__frame'
                title={`Карта: ${address}`}
                src={mapUrl}
                style={{ border: 0 }}
                allowFullScreen
                loading="lazy"
            ></iframe>
        </div>
    );
};

export {LocationMap};
