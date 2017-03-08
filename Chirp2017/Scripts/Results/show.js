$(document).ready(function () {
    $('.closeButton').click(function (e) {
            //debugger
            var $this = $(this);
            $this.parent().remove();
        })
})