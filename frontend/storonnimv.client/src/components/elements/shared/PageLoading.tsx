import { FC } from "react";
import { Container } from "react-bootstrap";

type LoadingProps = {
    width?: string; // Width of the loading container
    height?: string; // Height of the loading container
    elementsCount?: number; // Number of placeholder elements to display
    columns?: number; // Number of columns in the grid
};

const PageLoading: FC<LoadingProps> = ({ width = "100%", height = "100%", elementsCount = 9, columns = 3 }) => {
    return (
        <Container
            className="page-loading"
            style={{
                width, // Dynamic width
                height, // Dynamic height
                gridTemplateColumns: `repeat(${columns}, 1fr)`, // Dynamic columns
            }}
        >
            {Array.from({ length: elementsCount }).map((_, index) => (
                <div key={index} className="page-loading__placeholder"></div>
            ))}
        </Container>
    );
};

export { PageLoading };
