import {useEffect, useState} from "react";

const mediaQuery = "(prefers-reduced-motion: reduce)";

export function usePrefersReducedMotion(): boolean {
    const [prefersReducedMotion, setPrefersReducedMotion] = useState<boolean>(
        () => window.matchMedia(mediaQuery).matches
    );

    useEffect(() => {
        const media = window.matchMedia(mediaQuery);
        const handleChange = (event: MediaQueryListEvent) => setPrefersReducedMotion(event.matches);

        setPrefersReducedMotion(media.matches);
        media.addEventListener("change", handleChange);

        return () => media.removeEventListener("change", handleChange);
    }, []);

    return prefersReducedMotion;
}
