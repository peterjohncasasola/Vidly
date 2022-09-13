export class Customer {
    constructor(id, name, address, membershipTypeId, birthDate, isSubscribedToNewsLetter) {
        this.id = id;
        this.name = name;
        this.address = address;
        this.membershipTypeId = membershipTypeId;
        this.birthDate = birthDate;
        this.isSubscribedToNewsLetter = isSubscribedToNewsLetter;
    }
}