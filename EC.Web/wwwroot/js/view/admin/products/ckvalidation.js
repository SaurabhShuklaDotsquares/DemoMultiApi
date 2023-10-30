ignore: [];

    $(document).ready(function(){

        $("#LongDescription").validate(
            {
                ignore: [],
                debug: false,
                rules: {

                    cktext: {
                        required: function () {
                            CKEDITOR.instances.cktext.updateElement();
                        },

                        minlength: 250
                    }
                },
                messages:
                {

                    cktext: {
                        required: "Please enter Text",
                        minlength: "Please enter 250 characters"


                    }
                }
            });
        });
