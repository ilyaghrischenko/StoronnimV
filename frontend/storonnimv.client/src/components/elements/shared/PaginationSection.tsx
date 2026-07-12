import {FC} from "react";
// @ts-ignore
import ArrowIcon from "../../../assets/arrow-left.svg?react";
// @ts-ignore
import ArrowRightIcon from "../../../assets/arrow_right.svg?react";

interface IPaginationSectionProps {
    currentPage: number;
    totalPages: number;
    paginate: (pageNumber: number) => void;
}

const PaginationSection: FC<IPaginationSectionProps> =
    ({
         currentPage,
         totalPages,
         paginate,
     }) => {
        const getPageNumbers = (): (number | string)[] => {
            const pages: (number | string)[] = [];
            const maxVisiblePages = 5;
            const halfWindow = Math.floor(maxVisiblePages / 2);

            let startPage = Math.max(1, currentPage - halfWindow);
            const endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

            // корректировка, если не хватает страниц в конце
            if (endPage - startPage < maxVisiblePages - 1) {
                startPage = Math.max(1, endPage - maxVisiblePages + 1);
            }

            if (startPage > 1) {
                pages.push(1);
                if (startPage > 2) {
                    pages.push("...");
                }
            }

            for (let i = startPage; i <= endPage; i++) {
                pages.push(i);
            }

            if (endPage < totalPages) {
                if (endPage < totalPages - 1) {
                    pages.push("...");
                }
                pages.push(totalPages);
            }

            return pages;
        };

        return (
            <div className="pagination-container">
                <button
                    className="pagination-button"
                    onClick={() => paginate(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    <ArrowIcon className="pagination-button.next svg"/>
                </button>

                {getPageNumbers().map((item, index) => (
                    <button
                        key={index}
                        className={`pagination-button ${item === currentPage ? "active" : ""}`}
                        onClick={() => typeof item === 'number' && paginate(item)}
                        disabled={item === "..."}
                    >
                        {item}
                    </button>
                ))}

                <button
                    className="pagination-button"
                    onClick={() => paginate(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    <ArrowRightIcon/>
                </button>
            </div>
        );
    };

export {PaginationSection};
