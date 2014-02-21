(function($) {
$(document).ready(function () {

    // jQuery UI accordion init
    $('.accordion').accordion({
        heightStyle: 'content',
        collapsible: true
    });


    // jQuery UI tabs init
    $('.tabs').tabs();


    // Quote Rotator init
    if (jQuery().quoteRotator) {
        jQuery('#testimonial-content').quoteRotator({
            container: '#hidden-quotes',
            // the optional ease in array, set it to null if you want to use all the available animation
            easeInTypeArr: ['fadeIn', 'fadeInDown', 'fadeInUp', 'fadeInLeft', 'fadeInRight'],
            // the optional ease out array, set it to null if you want to use all the available animation
            easeOutTypeArr: ['fadeOutLeft', 'fadeOutRight', 'fadeOutUp', 'fadeOutDown'],
            // navigate to next quote or not when user click the quote
            clickToNext: false
        });
    }

    // Revolution slider init
    if (jQuery().revolution) {
        var tpj=jQuery;
        tpj.noConflict();

        if (tpj.fn.cssOriginal!=undefined)
            tpj.fn.css = tpj.fn.cssOriginal;

        tpj('.rev-slider').revolution({
            delay:9000,
            startHeight: 700,
            startWidth: 1170,

            onHoverStop: "on",

            navigationType: '',

            soloArrowLeftHalign:"left",
            soloArrowLeftValign:"center",
            soloArrowLeftHOffset:20,
            soloArrowLeftVOffset:50,

            soloArrowRightHalign:"right",
            soloArrowRightValign:"center",
            soloArrowRightHOffset:20,
            soloArrowRightVOffset:50,

            touchenabled:"on",

            stopAtSlide:-1,
            stopAfterLoops:-1,
            hideCaptionAtLimit: 0,
            hideAllCaptionAtLilmit: 0,
            hideSliderAtLimit:0,

            fullWidth:"on",
            fullScreen:"off",

            shadow:0
        });
    }

    //////////////////////
    // Magnific Popup init
    //////////////////////
    if (jQuery().magnificPopup) {
        jQuery('.thumbnail-hover a').magnificPopup({type:'image'});
    }

    /////////////////
    // Isotope Plugin
    /////////////////
    var container = jQuery('.isotope');
    var button = jQuery('.load-more-button');

    if (jQuery().imagesLoaded && jQuery().isotope) {

        // init the plugin
        container.imagesLoaded(function() {

            container.isotope({
                itemSelector : '.isotope-item'
            });

        });

        // reLayout the isotope plugin if the windows is resized
        $(window).smartresize(function(){
            container.isotope( 'reLayout');
        });

        // handle the addition of new data to the isotope container
        jQuery(button).on('click', function() {
            var elements = createMasonryElement();

            container.imagesLoaded(function(){
                container.append(elements).isotope('appended', elements);
            });
        });

        // handle the isotope filter
        jQuery('.isotope-filter ul li').click(function(){
            var selector = jQuery(this).attr('data-filter');
            container.isotope({ filter: selector });
            return false;
        });
    }

});
})(jQuery);
