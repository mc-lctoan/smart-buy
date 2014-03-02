
(function($) {

$(document).ready(function () {

    var header = $('#header-container');
    var offset = header.offset();
    var scrollUpButton = $('#scroll-up-button');
    var windowHeight = $(window).height();

    init();
    handlePhoneMenu();
    handleAccordionIcons();
    handleScrollToTopAnimation();
    handleHeaderOnScroll();
    handleScrollToTopButton();
    handleLatestPostsAnimations();
    handleSkillBars();

    // Init function
    function init() {

        // Header adjustment init
        if (offset.top > 5) {
            $(header).addClass('header-small');
        }
        else {
            $(header).removeClass('header-small');
        }


        // scroll-up button init position
        if (offset.top > windowHeight / 2) {
            scrollUpButton.removeClass('gone');
            scrollUpButton.addClass('visible');
        }
        else {
            scrollUpButton.removeClass('visible');
            scrollUpButton.addClass('gone');
        }


        // Prepare the animations for the inview elements
        /*---------  Inview Plugin + Animate.css  -------*/

        var fadeLeft = jQuery('.fadeLeft');
        var fadeRight = jQuery('.fadeRight');
        var fadeDown = jQuery('.fadeDown');
        var fadeIn = jQuery('.FadeIn');
        var fadeOut = jQuery('.FadeOut');

        fadeLeft.css({visibility: 'hidden'});
        fadeRight.css({visibility: 'hidden'});
        fadeDown.css({visibility: 'hidden'});
        fadeIn.css({visibility: 'hidden'});
        fadeOut.css({visibility: 'hidden'});

        var browser = false;
        p = navigator.platform;

        if (p === 'iPad' || p === 'iPhone' || p === 'iPod') {
            browser = true;
        }

        // Control Dynamic Content Sliding

        if (browser === false) {

            jQuery('.Fade').one('inview', function (event, visible) {
                if (visible) {
                    jQuery(this).addClass('animated fadeIn');
                    jQuery(this).css({visibility: 'visible'});
                }

            });

            jQuery('.FadeIn').one('inview', function (event, visible) {
                if (visible) {
                    jQuery(this).addClass('animated fadeInUp');
                    jQuery(this).css({visibility: 'visible'});
                }

            });

            jQuery('.fadeDown').on('inview', function(event, visible) {
                if (visible) {
                    jQuery(this).addClass('animated fadeInDown');
                    jQuery(this).css({visibility: 'visible'});
                }
            });

            fadeLeft.on('inview', function (event, visible) {
                if (visible) {
                    jQuery(this).addClass('animated fadeInLeft');
                    jQuery(this).css({visibility: 'visible'});
                }
            });
            fadeLeft.unbind('inview', function (event, visible) {
                if (visible) {
                    jQuery(this).removeClass('animated fadeInLeft');
                }
            });
            fadeRight.on('inview', function (event, visible) {
                if (visible) {
                    jQuery(this).css({visibility: 'visible'});
                    jQuery(this).addClass('animated fadeInRight');
                }
            });
            fadeRight.unbind('inview', function (event, visible) {
                if (visible) {
                    jQuery(this).removeClass('animated fadeInRight');
                }
            });
        }
    }


    // Phone Menu Handler
    function handlePhoneMenu() {
        var expanded = false;
        var phoneMenuFields = $('.phone-menu-fields');
        var phoneMenuFieldsHeight = phoneMenuFields.height();

        phoneMenuFields.css({height: 0});

        $('.phone-menu-toggle').on('click', function() {
            if (expanded === false) {
                phoneMenuFields.animate({height: phoneMenuFieldsHeight}, 500);
                phoneMenuFields.css({display: 'block'});
                expanded = true;
            }
            else {
                phoneMenuFields.animate({height: 0});
                expanded = false;
            }
        });
    }


    // Handler for the accordion icons when the state is changed
    function handleAccordionIcons() {
        $('.accordion h5.ui-state-active i').removeClass('icon-plus-sign').addClass('icon-minus-sign');

        $('.accordion h5').click(function() {
            jQuery('.accordion').children('h5').each(function(i) {
                if (jQuery(this).hasClass('ui-state-active')) {
                    jQuery(this).children('i').removeClass('icon-plus-sign');
                    jQuery(this).children('i').addClass('icon-minus-sign');
                }
                else {
                    jQuery(this).children('i').removeClass('icon-minus-sign');
                    jQuery(this).children('i').addClass('icon-plus-sign');
                }
            });
        });
    }


    // scroll to top handler
    function handleScrollToTopAnimation() {
        $('#scroll-up-button').click(function(){
            $('html, body').animate({scrollTop:0}, 'slow');
            return false;
        });
    }


    // handles the header pinning when the user scrolls down
    function handleHeaderOnScroll() {
        $(document).scroll(function() {

            var header = $('#header-container');

            var offset = header.offset();

            if (offset.top > 5) {
                $(header).addClass('header-small');
            }
            else {
                $(header).removeClass('header-small');
            }
        });
    }


    // handles scroll-to-top button visibility
    function handleScrollToTopButton() {
        $(document).scroll(function() {
            var offset = header.offset();
            if (offset.top > windowHeight / 2) {
                scrollUpButton.removeClass('gone');
                scrollUpButton.addClass('visible');
            }
            else {
                scrollUpButton.removeClass('visible');
                scrollUpButton.addClass('gone');
            }
        });
    }

    // handles latest post hover animations
    function handleLatestPostsAnimations() {
        var postWidth = 0;
        var pixelsToMove = 0;
        var postHeight = 0;

        var firstPostLeftPosition = 270;
        var lastPostLeftPosition = 570;

        var isHovered = false;

        var firstLatestPost = jQuery('.latest-post');
        var lastLatestPost = jQuery('.latest-post:last-child');

        jQuery('.latest-posts .posts div div:first-child .post .post-media').hover(function() {

                firstLatestPost.css({
                    left: parseInt(firstPostLeftPosition) + 300 + 'px'
                });

                lastLatestPost.css({
                    left: parseInt(lastPostLeftPosition) + 300 + 'px'
                });

        });

        firstLatestPost.hover(function() {
            firstLatestPost.css({
                left: '270px'
            });

            lastLatestPost.css({
                left: parseInt(lastPostLeftPosition) + 300 + 'px'
            });
        });

        lastLatestPost.hover(function() {
            firstLatestPost.css({
                left: '270px'
            });

            lastLatestPost.css({
                left: '570px'
            })
        });
    }

    // handles skill bars animation
    function handleSkillBars() {
        $('.skill-bars-wrapper').on('inview', function(event, visible) {
            if (visible) {
                var skillBarWidth = 0;

                $(this).find('.skill-bar-fill').each(function(i) {
                    skillBarWidth = $(this).attr('data-width');

                    $(this).css('width', skillBarWidth + '%');
                });
            }
        });
    }

});

})(jQuery);
