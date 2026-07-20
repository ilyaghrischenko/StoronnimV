import { FC } from "react";

const SpotifyContainer: FC = () => {
    return (
        <div className="spotify-container">
            <iframe
                className="spotify-container__frame"
                src="https://open.spotify.com/embed/artist/6NTzEgUmN1PIBIYEHhf1kS?utm_source=generator&theme=0"
                frameBorder="0"
                allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                loading="lazy"
                title="Spotify — Стороннім В"
            />
        </div>
    );
};

export { SpotifyContainer };
