export interface ODataResponse<Type> {
    value: Type;
    // eslint-disable-next-line @typescript-eslint/naming-convention
    "@odata.count": number;
}
