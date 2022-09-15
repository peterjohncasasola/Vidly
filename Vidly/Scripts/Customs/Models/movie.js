export class Movie {
    constructor(id, name, genre, stock, dateRelease, minimumRequiredAge) {
        this.id = id;
        this.name = name;
        this.genre = genre;
        this.dateRelease = dateRelease;
        this.stock = stock;
        this.minimumRequiredAge = minimumRequiredAge;
    }
}