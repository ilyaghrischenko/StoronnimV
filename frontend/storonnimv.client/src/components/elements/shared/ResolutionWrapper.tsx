import {FC, ReactNode} from "react";
import {MobileInDeveloping} from "../../pages/MobileInDeveloping.tsx";
import {useMediaQuery} from "@mui/material";

interface IResolutionWrapperProps {
    children: ReactNode;
}

const ResolutionWrapper: FC<IResolutionWrapperProps> = ({ children }) => {
    const isMobile = useMediaQuery("(max-width: 980px)");

    if (isMobile) return <MobileInDeveloping />;

    return <>{children}</>;
};

export { ResolutionWrapper };