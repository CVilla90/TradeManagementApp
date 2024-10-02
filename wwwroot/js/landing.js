/* TradeManagementApp\wwwroot\js\landing.js */

document.addEventListener('DOMContentLoaded', function () {
    // Initialize Bootstrap 5 carousel with the correct configuration
    var carouselElement = document.querySelector('#heroCarousel');
    var carousel = new bootstrap.Carousel(carouselElement, {
        interval: 5000,  // 5 seconds interval for carousel
        wrap: true,      // Ensures that the carousel loops back to the first image after the last
        ride: 'carousel' // Automatically cycles through the images
    });

    // Initialize AOS (Animate On Scroll)
    AOS.init();

    // Click-to-expand functionality for service boxes
    document.querySelectorAll('.service-box').forEach(function (box) {
        box.addEventListener('click', function () {
            box.classList.toggle('expanded');
        });
    });
});
