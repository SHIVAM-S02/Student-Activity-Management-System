const slider = document.querySelector(".slider");
const slides = document.querySelectorAll(".slide");
let counter = 0;

function nextSlide() {
    if (counter >= slides.length - 1) {
        counter = 0;
    } else {
        counter++;
    }
    updateSlider();
}

function updateSlider() {
    slider.style.transform = `translateX(-${counter * 100}%)`;
}

setInterval(nextSlide, 3000); // Auto slide every 3 seconds