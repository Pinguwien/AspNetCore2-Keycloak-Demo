// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
<script>
$(function () {

    $("#call1").click(function(e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr("formaction"),
            type: "GET",
        }).done(function(data) {
            $('#callresults').innerHTML(data);
        }).fail(function(a, v, e) {
            $('#callresults').innerHTML(e);
        });
    });
})
</script>