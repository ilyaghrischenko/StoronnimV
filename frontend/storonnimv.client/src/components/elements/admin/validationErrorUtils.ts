const normalizeFieldName = (fieldName: string): string => fieldName
    .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
    .replace(/[^a-zA-Z0-9_-]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();

const getValidationErrorId = (idPrefix: string, fieldName: string): string =>
    `${idPrefix}-${normalizeFieldName(fieldName)}`;

const hasValidationError = (errors: Record<string, string[]>, fieldName: string): boolean =>
    Boolean(errors[fieldName]?.length);

export {getValidationErrorId, hasValidationError};
