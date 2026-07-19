import {FC} from "react";
// @ts-expect-error vite-plugin-svgr resolves the React component query during the Vite build.
import ArrowIcon from "../../../assets/arrow-left.svg?react";
// @ts-expect-error vite-plugin-svgr resolves the React component query during the Vite build.
import ArrowRightIcon from "../../../assets/arrow_right.svg?react";

interface IPaginationSectionProps {
    currentPage: number;
    totalPages: number;
    paginate: (pageNumber: number) => void;
    compactOnMobile?: boolean;
}

const PaginationSection: FC<IPaginationSectionProps> =
    ({
         currentPage,
         totalPages,
         paginate,
         compactOnMobile = false,
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
            <nav
                className={`pagination-container ${compactOnMobile ? "pagination-container--compact-mobile" : ""}`}
                aria-label="Пагінація"
            >
                <button
                    type="button"
                    aria-label="Попередня сторінка"
                    className="pagination-button"
                    onClick={() => paginate(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    <ArrowIcon className="pagination-button.next svg"/>
                </button>

                <div className="pagination-container__pages">
                    {getPageNumbers().map((item, index) =>
                        typeof item === "number" ? (
                            <button
                                type="button"
                                key={item}
                                aria-label={`Сторінка ${item}`}
                                aria-current={item === currentPage ? "page" : undefined}
                                className={`pagination-button ${item === currentPage ? "active" : ""}`}
                                onClick={() => paginate(item)}
                            >
                                {item}
                            </button>
                        ) : (
                            <span className="pagination-ellipsis" key={`ellipsis-${index}`} aria-hidden="true">
                                {item}
                            </span>
                        )
                    )}
                </div>

                <span className="pagination-container__compact-status" aria-current="page">
                    {currentPage} / {totalPages}
                </span>

                <button
                    type="button"
                    aria-label="Наступна сторінка"
                    className="pagination-button"
                    onClick={() => paginate(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    <ArrowRightIcon/>
                </button>
            </nav>
        );
    };

export {PaginationSection};
