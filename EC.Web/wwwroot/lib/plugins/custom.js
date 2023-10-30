
$('.slide-next-key').owlCarousel({
    items: 4,
    loop: true,
    margin: 40,
    dots: false,
    autoplay: true,
    autoplayTimeout: 2000,
    autoplayHoverPause: true,
    responsive: {
        0: {
            items: 1,
            margin: 8,
        },
        600: {
            items: 4,
        },
        1000: {
            items: 5,
        }
    }
});


class Carousel {
    constructor(params) {
        this.carouselContainer = params.container;
        this.controlsContainer = params.controlsContainer;
        this.controlsClassName = this.controlsContainer.className
        this.displayControls = params.displayControls;
        this.carouselControls = ['previous', 'next'];
        this.carouselArray = [...params.items];
        this.classNameItem = this.carouselArray[0].className;
        this.carouselLength = this.carouselArray.length;
        this.textControls = params.textControls;
        this.autoplay = params.autoplay;
        this.autoplayTime = params.autoplayTime;
        this.setParams()
    }


    getCurrentState() {
        const selectedItem = this.carouselContainer.querySelector(`.${this.classNameItem}-selected`);
        const previousSelectedItem = this.carouselContainer.querySelector(`.${this.classNameItem}-previous`);
        const nextSelectedItem = this.carouselContainer.querySelector(`.${this.classNameItem}-next`);
        const firstCarouselItem = this.carouselContainer.querySelector(`.${this.classNameItem}-first`);
        const lastCarouselItem = this.carouselContainer.querySelector(`.${this.classNameItem}-last`);

        var indexLastCarouselItem = parseInt(lastCarouselItem.getAttribute('data-index'))
        var indexFirstCarouselItem = parseInt(firstCarouselItem.getAttribute('data-index'))
        var indexDownCarouselItem = indexFirstCarouselItem - 1
        var indexUpCarouselItem = indexLastCarouselItem + 1

        const downCommingCarouselItem = this.carouselContainer.querySelector(`.${this.classNameItem}[data-index='${indexDownCarouselItem}']`);
        const upCommingCarouselItem = this.carouselContainer.querySelector(`.${this.classNameItem}[data-index='${indexUpCarouselItem}']`);

        return [selectedItem, previousSelectedItem, nextSelectedItem, firstCarouselItem, lastCarouselItem, downCommingCarouselItem, upCommingCarouselItem, indexDownCarouselItem, indexUpCarouselItem]
    }

    //init all params
    setParams() {

        this.setInitialState();
        this.setControls();
        this.onTouch();

        if (this.displayControls) {
            this.useControls();
        }
        if (this.autoplay) {
            setInterval(function () { this.setCurrentState(this.controlsContainer.childNodes[1], this.getCurrentState()) }.bind(this), this.autoplayTime)
        }
    }

    // Construct the carousel controls
    setControls() {
        this.carouselControls.forEach((control, index) => {
            this.controlsContainer.appendChild(document.createElement('button')).className = `${this.controlsClassName}-${control}`;
        });
        if (this.displayControls) {
            !!this.controlsContainer.childNodes[0] ? this.controlsContainer.childNodes[0].insertAdjacentHTML('beforeend', this.textControls[0]) : null;
            !!this.controlsContainer.childNodes[1] ? this.controlsContainer.childNodes[1].insertAdjacentHTML('beforeend', this.textControls[1]) : null;
        }

    }

    // Assign initial css classes and attribute for each items
    setInitialState() {
        this.carouselArray.forEach((item, index) => {
            item.setAttribute('data-index', index)
        })
        this.carouselArray[0].classList.add(`${this.classNameItem}-first`);
        this.carouselArray[1].classList.add(`${this.classNameItem}-previous`);
        this.carouselArray[2].classList.add(`${this.classNameItem}-selected`);
        this.carouselArray[3].classList.add(`${this.classNameItem}-next`);
        this.carouselArray[4].classList.add(`${this.classNameItem}-last`);
    }

    // Update the order state of the carousel with css classes
    setCurrentState(target, [selected, previous, next, first, last, downComming, upComming, downIndex, upIndex]) {

        selected.classList.remove(`${this.classNameItem}-selected`);
        previous.classList.remove(`${this.classNameItem}-previous`);
        next.classList.remove(`${this.classNameItem}-next`);
        first.classList.remove(`${this.classNameItem}-first`);
        last.classList.remove(`${this.classNameItem}-last`);

        if (target.className == `${this.controlsClassName}-${this.carouselControls[1]}`) {
            this.carouselArray.forEach((item) => {
                item.classList.remove(`${this.classNameItem}-trigger-previous`)
                item.classList.add(`${this.classNameItem}-trigger-next`)
            })
            switch (upIndex) {
                case this.carouselLength:
                    this.carouselArray[0].classList.add(`${this.classNameItem}-last`);
                    previous.classList.add(`${this.classNameItem}-first`);
                    selected.classList.add(`${this.classNameItem}-previous`);
                    next.classList.add(`${this.classNameItem}-selected`);
                    last.classList.add(`${this.classNameItem}-next`);
                    break
                case 1:
                    this.carouselArray[0].classList.add(`${this.classNameItem}-next`);
                    this.carouselArray[1].classList.add(`${this.classNameItem}-last`);
                    previous.classList.add(`${this.classNameItem}-first`);
                    selected.classList.add(`${this.classNameItem}-previous`);
                    next.classList.add(`${this.classNameItem}-selected`);
                    break
                case 2:
                    this.carouselArray[0].classList.add(`${this.classNameItem}-selected`);
                    this.carouselArray[1].classList.add(`${this.classNameItem}-next`);
                    this.carouselArray[2].classList.add(`${this.classNameItem}-last`);
                    previous.classList.add(`${this.classNameItem}-first`);
                    selected.classList.add(`${this.classNameItem}-previous`);
                    break
                case 3:
                    this.carouselArray[0].classList.add(`${this.classNameItem}-previous`);
                    this.carouselArray[1].classList.add(`${this.classNameItem}-selected`);
                    this.carouselArray[2].classList.add(`${this.classNameItem}-next`);
                    this.carouselArray[3].classList.add(`${this.classNameItem}-last`);
                    previous.classList.add(`${this.classNameItem}-first`);
                    break
                case 4:
                    this.setInitialState()
                    break
                default:
                    previous.classList.add(`${this.classNameItem}-first`);
                    selected.classList.add(`${this.classNameItem}-previous`);
                    next.classList.add(`${this.classNameItem}-selected`);
                    last.classList.add(`${this.classNameItem}-next`);
                    upComming.classList.add(`${this.classNameItem}-last`)
            }
        }

        else {
            this.carouselArray.forEach((item) => {
                item.classList.remove(`${this.classNameItem}-trigger-next`)
                item.classList.add(`${this.classNameItem}-trigger-previous`)
            })
            switch (downIndex) {
                case -1:
                    this.carouselArray[(this.carouselLength - 1)].classList.add(`${this.classNameItem}-first`);
                    first.classList.add(`${this.classNameItem}-previous`);
                    previous.classList.add(`${this.classNameItem}-selected`);
                    selected.classList.add(`${this.classNameItem}-next`);
                    next.classList.add(`${this.classNameItem}-last`);
                    break
                case (this.CarouselLength - 2):
                    this.carouselArray[this.carouselLength - 1].classList.add(`${this.classNameItem}-previous`);
                    this.carouselArray[this.carouselLength - 2].classList.add(`${this.classNameItem}-first`);
                    previous.classList.add(`${this.classNameItem}-selected`);
                    selected.classList.add(`${this.classNameItem}-next`);
                    next.classList.add(`${this.classNameItem}-last`);
                    break
                case (this.CarouselLength - 3):
                    this.carouselArray[this.carouselLength - 1].classList.add(`${this.classNameItem}m-selected`);
                    this.carouselArray[this.carouselLength - 2].classList.add(`${this.classNameItem}-previous`);
                    this.carouselArray[this.carouselLength - 3].classList.add(`${this.classNameItem}m-first`);
                    selected.classList.add(`${this.classNameItem}-next`);
                    next.classList.add(`${this.classNameItem}-last`);
                    break
                case (this.CarouselLength - 4):
                    this.carouselArray[this.carouselLength - 1].classList.add(`${this.classNameItem}-next`);
                    this.carouselArray[this.carouselLength - 2].classList.add(`${this.classNameItem}m-selected`);
                    this.carouselArray[this.carouselLength - 3].classList.add(`${this.classNameItem}-previous`);
                    this.carouselArray[this.carouselLength - 4].classList.add(`${this.classNameItem}-first`);
                    selected.classList.add(`${this.classNameItem}-next`);
                    next.classList.add(`${this.classNameItem}-last`);
                    break
                default:
                    downComming.classList.add(`${this.classNameItem}-first`)
                    first.classList.add(`${this.classNameItem}-previous`);
                    previous.classList.add(`${this.classNameItem}-selected`);
                    selected.classList.add(`${this.classNameItem}-next`);
                    next.classList.add(`${this.classNameItem}-last`);
            }
        }
    }

    useControls() {
        this.controlsContainer.childNodes.forEach(control => {
            control.addEventListener('click', () => {
                const target = control;
                this.setCurrentState(target, this.getCurrentState());
            });
        });
    }

    // touch action
    onTouch() {
        let touchstartX = 0;
        let touchendX = 0;
        let clickX = 0;
        let drag = false;

        this.carouselContainer.addEventListener('mousedown', function (event) {
            clickX = event.pageX
            drag = true
        }, false)

        this.carouselContainer.addEventListener('mouseup', function (event) {
            if (drag) {
                if (event.pageX < clickX) {
                    this.setCurrentState(this.controlsContainer.childNodes[1], this.getCurrentState())
                } else if (event.pageX > clickX) {
                    this.setCurrentState(this.controlsContainer.childNodes[0], this.getCurrentState())
                }
            }
        }.bind(this), false)
        this.carouselContainer.addEventListener('touchstart', function (event) {
            touchstartX = event.changedTouches[0].screenX;
        }, false);
        this.carouselContainer.addEventListener('touchend', function (event) {
            touchendX = event.changedTouches[0].screenX;
            if (touchendX <= touchstartX) {
                this.setCurrentState(this.controlsContainer.childNodes[1], this.getCurrentState())
            }
            else if (touchendX > touchstartX) {
                this.setCurrentState(this.controlsContainer.childNodes[0], this.getCurrentState())
            }
        }.bind(this), false);
    }
}


const myCarousel = new Carousel({
    container: document.querySelector('.slider'),
    items: document.querySelectorAll('.slider__item'),
    displayControls: true,
    controlsContainer: document.querySelector('.slider__controls'),
    textControls: ["<i class='fas fa-chevron-left'></i>", "<i class='fas fa-chevron-right'></i>"],
    autoplay: false,
    autoplayTime: 3500
});




$('.slide-gallery').owlCarousel({
    loop: true,
    margin: 30,
    responsiveClass: true,
    nav: true,
    dots: false,
    autoplay: true,
    autoplayTimeout: 1500,
    autoplayHoverPause: true,
    responsive: {
        0: {
            items: 2,
            margin: 8,
        },
        600: {
            items: 5,
            margin: 10,
        },
        1000: {
            items: 5,
        }
    }
});

$(document).ready(function () {
    
    var separation;
    if (screen.width > 1100 && screen.width <= 1366) {
        separation = 90;
    }
    else if (screen.width > 1366 && screen.width <= 1600) {
        separation = 130;
    }
    else {
        separation = 145;
    }

    var carousel = setTimeout(
        function () {
            $(".video-show-slide").waterwheelCarousel({
                orientation: 'vertical',
                separation: separation,
                opacityMultiplier: .4,
                flankingItems: 3,
                preloadImages: true,
                flankingItems: 3

            });
        },
        3000);

    $('#prev').bind('click', function () {
        carousel.prev();
        return false
    });

    $('#next').bind('click', function () {
        carousel.next();
        return false;
    });

    $('#reload').bind('click', function () {
        newOptions = eval("(" + $('#newoptions').val() + ")");
        carousel.reload(newOptions);
        return false;
    });

    $('.select-video-slide1').click(function () {
        $('.video-show-slide a img').addClass('editable');
        $('.list-tab li a.active').removeClass('active');
        $('.select-video-slide1').addClass("active");
        $('#item-1').trigger('click');
        $('#item-1').removeClass('editable');
    });
    $('.select-video-slide2').click(function () {
        $('.video-show-slide a img').addClass('editable');
        $('.list-tab li a.active').removeClass('active');
        $('.select-video-slide2').addClass("active");
        $('#item-2').trigger('click');
        $('#item-2').removeClass('editable');
    });
    $('.select-video-slide3').click(function () {
        $('.video-show-slide a img').addClass('editable');
        $('.list-tab li a.active').removeClass('active');
        $('.select-video-slide3').addClass("active");
        $('#item-3').trigger('click');
        $('#item-3').removeClass('editable');
    });
    $('.select-video-slide4').click(function () {
        $('.video-show-slide a img').addClass('editable');
        $('.list-tab li a.active').removeClass('active');
        $('.select-video-slide4').addClass("active");
        $('#item-4').trigger('click');
        $('#item-4').removeClass('editable');
    });
    $('.select-video-slide5').click(function () {
        $('.video-show-slide a img').addClass('editable');
        $('.list-tab li a.active').removeClass('active');
        $('.select-video-slide5').addClass("active");
        $('#item-5').trigger('click');
        $('#item-5').removeClass('editable');
    });

    $(window).on('scroll', function () {
        var scrollTop = $(window).scrollTop(),
            docHeight = $(document).height(),
            windowHeight = $(window).height(),
            elementTop = $(".track-block").offset().top,
            headerHeight = $(".header").innerHeight();

        scrollPercent = (scrollTop / (docHeight - windowHeight)) * (elementTop + 100 + headerHeight);    //900;
        //var curdiv = $(".excersise-bar1").offset();
        var curdiv = $(".header").innerHeight();
        //console.log(curdiv);

        var position = scrollPercent;
        if (position >= 100) {
            $(".excersise-bar1").css('width', '100%');
            $('.bar-percentage1').text('100%');
            $('.tracking-bar1').css({ opacity: 1 });
            $('.bar-percentage1').css('-webkit-text-stroke', ' 1.5px #fff');
            $('.excersise-bar1').parent().css({ borderColor: '#ed2c7e' });
            if (position >= 110) {
                $(".excersise-bar2").css('width', '100%');
                $('.bar-percentage2').text('100%');
                $('.bar-percentage2').css('-webkit-text-stroke', ' 1.5px #fff');
                $('.tracking-bar2').css({ opacity: 1 });
                $('.line1').css({ background: '#ed2c7e' });
                $('.excersise-bar2').parent().css({ borderColor: '#ed2c7e' });
            } else {
                $(".excersise-bar2").css('width', (position - 10) + '%');
                $('.bar-percentage2').text(parseInt(position - 10) + '%');
                $('.tracking-bar2').css({ opacity: 0 });
                $('.excersise-bar2').parent().css({ borderColor: '#ef99b9' });
                $('.line1').css({ background: '#ef99b9' });
                $('.bar-percentage2').css('-webkit-text-stroke', ' 1.5px #000');
            }
            if (position >= 120) {
                $(".excersise-bar3").css('width', '100%');
                $('.bar-percentage3').text('100%');
                $('.bar-percentage3').css('-webkit-text-stroke', ' 1.5px #fff');
                $('.tracking-bar3').css({ opacity: 1 });
                $('.line2').css({ background: '#ed2c7e' });
                $('.excersise-bar3').parent().css({ borderColor: '#ed2c7e' });
            } else {
                $(".excersise-bar3").css('width', (position - 20) + '%');
                $('.bar-percentage3').text(parseInt(position - 20) + '%');
                $('.tracking-bar3').css({ opacity: 0 });
                $('.bar-percentage3').css('-webkit-text-stroke', ' 1.5px #000');
                $('.excersise-bar3').parent().css({ borderColor: '#ef99b9' });
                $('.line2').css({ background: '#ef99b9' });
            }
        } else {
            $(".excersise-bar1").css('width', position + '%');
            $(".excersise-bar2").css('width', (position - 10) + '%');
            $(".excersise-bar3").css('width', (position - 20) + '%');
            if (position < 0) {
                $('.bar-percentage1').text(parseInt(0) + '%');
            } else {
                $('.bar-percentage1').text(parseInt(position) + '%');
            }            
            if (position < 10) {
                $('.bar-percentage2').text(parseInt(0) + '%');
            }
            else {
                $('.bar-percentage2').text(parseInt(position - 10) + '%');
            }

            if (position < 20) {
                $('.bar-percentage3').text(parseInt(0) + '%');
            }
            else {
                $('.bar-percentage3').text(parseInt(position - 20) + '%');
            }
            //$('.bar-percentage3').text(parseInt(position - 20) + '%');
            $('.tracking-bar1,.tracking-bar2,.tracking-bar3').css({ opacity: 0 });
            $('.run-baar-box').css({ borderColor: '#ef99b9' });
            $('.line1,.line2').css({ background: '#ef99b9' });
            $('.bar-percentage1').css('-webkit-text-stroke', ' 1.5px #000');
        }
        

        //For earning bar
        var top_of_element = $(".earn-block").offset().top;
        var bottom_of_element = $(".earn-block").offset().top + $(".earn-block").outerHeight();
        var bottom_of_screen = $(window).scrollTop() + $(window).innerHeight();
        var top_of_screen = $(window).scrollTop();


        $('.video-show-slide a img:not(.carousel-center)').addClass('editable');
        $('.carousel-center').removeClass('editable');
        //

        var top_of_element = $(".testimonies-block").offset().top;
        var payment_top = $(".earn-block").offset().top;
        var blockheight = payment_top - top_of_element;
        if (scrollTop >= top_of_element) {
            var elementScroll = scrollTop - top_of_element;
            var percent = parseInt(elementScroll / blockheight * 100);
            if (percent > 80) {
                $(".pro-bag").show();
                percent = 89;
            }
            else {
                $(".pro-bag").hide();
            }
            //$('.pro-bag').hide();
            $('.fill-uper-line').css({ width: percent + "%" });
        }
    });
});

jQuery(function ($) {
    'use strict',
        new WOW().init();
});