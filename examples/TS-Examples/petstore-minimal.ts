// This file was autogenerated by nSwagger 0.0.1 - changes made to it maybe lost if nSwagger is run again
namespace nSwagger {
    export module SwaggerPetstore {
        export class Pet {
            id: number;
            name: string;
            tag: string;
        }

        export interface API {
            setToken(value:string, headerOrQueryName:string, isQuery:boolean):void;
            (): PromiseLike<Pet>;
        }
    }
}