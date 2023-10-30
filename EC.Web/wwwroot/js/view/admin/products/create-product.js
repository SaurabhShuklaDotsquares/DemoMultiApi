(function ($) {
    function Cms() {
        var $this = this, $formAddEditproduct;
        var arrImage_remove_store = new Array();
        var isMergeButtonClicked = 0;
        function initializeGrid() {
            ckEditor();

            document.querySelectorAll(".btn-navigate-form-step").forEach((formNavigationBtn) => {
                formNavigationBtn.addEventListener("click", () => {

                    var returnvalue = true;
                    const stepNumber = parseInt(formNavigationBtn.getAttribute("step_number"));
                    var regex = /&(nbsp|amp|quot|lt|gt);/g;
                    if ($('#LongDescription').val().replace(regex, "").replace('<p>', '').replace('</p>\n', '').trim() == "") {
                        $('.errLongDescription').html('Please enter product description');
                        /*returnvalue = false;*/
                        //return false;
                    }
                    else {
                        $('.errLongDescription').html('');
                        /* returnvalue = true;*/
                    }

                    if ($("form").valid()) {

                        if (checkRequiredField(stepNumber)) {

                            navigateToFormStep(stepNumber);
                        }
                        $("#LongDescription").val(CKEDITOR.instances['LongDescription'].getData())

                        if (stepNumber == 3) {

                            if ($("#option_data").val() != "") {

                                var attribute = [];
                                var attributevalue = [];

                                obj = $("#option_data").val()
                                $.each(obj, function (key, value) {
                                    attribute.push(value);
                                });

                                for (var i = 0; i < attribute.length; i++) {
                                    var data = $("#attribute_options_" + attribute[i]).val()
                                    attributevalue.push(data);
                                }
                                $("#hdnAttributeValues").val(JSON.stringify(attributevalue));
                            }
                        }
                        else if (stepNumber == 4) {

                            if (ImageArray.length > 0) {
                                $("#hdnAttributeImages").val(ImageArray);
                            }
                        }
                        return returnvalue
                    }
                    else {
                        returnvalue = false;
                        return returnvalue;
                    }
                });
            });

            $(".attribute-select").select2();
            getVariants();
            mergeAttributes();
            clearAttributes();
            $(document).off('change', '#ProductType').on('change', '#ProductType', function () {

                if ($(this).val() == "Configuration") {
                    $("#divprice").css('display', 'none');
                    $("#Discountprice").css('display', 'none');
                }
                else {
                    $("#divprice").css('display', 'block');
                    $("#Discountprice").css('display', 'block');
                }
            });

            var ImageArray = [];
            //$(document).off('change', '.btnUploadFile').on('change', '.btnUploadFile', function (e) {

            //    if ($(this).val() != "") {

            //        var file = e.target.files[0].name;
            //        var ImageId = $(this).attr('id');
            //        var CurrentImagename = file +"_"+ ImageId
            //        var ImageExist = false;

            //        if (ImageArray.length > 0) {

            //            $.each(ImageArray, function (index, val) {

            //                if (val.includes(ImageId)) {
            //                    ImageExist = true;
            //                    ImageArray[index] = CurrentImagename;
            //                }

            //            });

            //            if (!ImageExist) {

            //                ImageArray.push(CurrentImagename)
            //            }


            //        }
            //        else {
            //            ImageArray.push(CurrentImagename)

            //        }

            //    }

            //});

            //$(document).off('change', '#MyImage').on('change', '#MyImage', function (e) {

            //    var myfile = document.getElementById("MyImage");
            //    var formData = new FormData();
            //    var FileName = e.target.files[0];

            //    var files = $('#MyImage')[0].files[0];
            //    formData.append('MyImage', files);

            //});




            function navigateToFormStep(stepNumber) {

                document.querySelectorAll(".form-step").forEach((formStepElement) => {
                    formStepElement.classList.add("d-none");
                });
                document.querySelectorAll(".form-stepper-list").forEach((formStepHeader) => {
                    formStepHeader.classList.add("form-stepper-unfinished");
                    formStepHeader.classList.remove("form-stepper-active", "form-stepper-completed");
                });
                document.querySelector("#step-" + stepNumber).classList.remove("d-none");
                const formStepCircle = document.querySelector('li[step="' + stepNumber + '"]');
                formStepCircle.classList.remove("form-stepper-unfinished", "form-stepper-completed");
                formStepCircle.classList.add("form-stepper-active");
                for (let index = 0; index < stepNumber; index++) {
                    const formStepCircle = document.querySelector('li[step="' + index + '"]');
                    if (formStepCircle) {
                        formStepCircle.classList.remove("form-stepper-unfinished", "form-stepper-active");
                        formStepCircle.classList.add("form-stepper-completed");
                    }
                }
            };


            function getVariants() {
                const attributeValues = (index, title) => '<div class="variation_options_' + index + '"><div class="form-group row mx-sm-3"><label style="margin-right: 10px">' + title + '</label><div class="col-sm-5"><select class="attributeVariants form-control" style="width:50%;" multiple="multiple" name="attribute_options[' + index + ']" required data-title= "' + title + '" id="attribute_options_' + index + '"><option value="" disabled>Select Attribute Values</option></select><span class="text-danger" id="attribute_options_error_' + index + '"></span></div></div></div>';

                $('.variant_button').click(function () {

                    let option = $('#option_data').val();

                    $.ajax({
                        url: '/admin/products/GetOptionValues',
                        type: "POST",
                        data: {
                            attribute_id: option
                        },
                        success: function (data) {

                            let optionTitleList = [];
                            $("#option_data option:selected").each(function () {
                                var $this = $(this);
                                if ($this.length) {
                                    optionTitle = $this.text();

                                    optionTitleList.push(optionTitle);
                                }
                            });
                            if (data) {
                                $('#attributeValues').html("");

                                var option1 =
                                    $.each(option, function (index, optionId) {
                                        $('.merge_attribute').show();
                                        $('.clear_attribute').attr('style', '');

                                        var title = optionTitleList[index];
                                        $('.attribute_options').attr('style', 'display:none');
                                        $('.clear_button').show();

                                        $('#attributeValues').append(attributeValues(optionId, title));
                                        $('.attributeVariants').select2();

                                        for (let i = 0; i < data.optionValuesList[index].length; i++) {
                                            $('#attribute_options_' + optionId).append(
                                                $('<option value="' + data.optionValuesList[index][i].value + '">' + data.optionValuesList[index][i].text + '</option>')
                                            );
                                        }
                                    });
                            }
                            var Attrvalues = JSON.parse($('#AttributeValuse').val());
                            if (Attrvalues != null && Attrvalues.length > 0) {
                                $.each(Attrvalues, function (index, value) {

                                    $('#attribute_options_' + value.attributeId).val(value.attributeValue).change();
                                });
                                $('.merge_button').trigger('click');
                            }
                        },
                        error: function (data) { }
                    });
                })


                $('#option_data').change(function () {

                    let option = $('#option_data').val();

                    $.ajax({
                        url: '/admin/products/GetOptionValues',
                        type: "POST",
                        data: {
                            attribute_id: option
                        },
                        success: function (data) {

                            let optionTitleList = [];
                            $("#option_data option:selected").each(function () {
                                var $this = $(this);
                                if ($this.length) {
                                    optionTitle = $this.text();

                                    optionTitleList.push(optionTitle);
                                }
                            });
                            if (data) {
                                $('#attributeValues').html("");

                                var option1 =
                                    $.each(option, function (index, optionId) {
                                        $('.merge_attribute').show();
                                        $('.clear_attribute').attr('style', '');

                                        var title = optionTitleList[index];
                                        $('.attribute_options').attr('style', 'display:none');
                                        $('.clear_button').show();

                                        $('#attributeValues').append(attributeValues(optionId, title));
                                        $('.attributeVariants').select2();

                                        for (let i = 0; i < data.optionValuesList[index].length; i++) {
                                            $('#attribute_options_' + optionId).append(
                                                $('<option value="' + data.optionValuesList[index][i].value + '">' + data.optionValuesList[index][i].text + '</option>')
                                            );
                                        }
                                    });
                            }
                            var Attrvalues = JSON.parse($('#AttributeValuse').val());
                            if (Attrvalues != null && Attrvalues.length > 0) {
                                $.each(Attrvalues, function (index, value) {

                                    $('#attribute_options_' + value.attributeId).val(value.attributeValue).change();
                                });
                                $('.merge_button').trigger('click');
                            }
                        },
                        error: function (data) { }
                    });
                })


                //$('.attributeVariants').change(function () {
                //    alert("dd");
                //    $('.merge_attribute').show();
                //})
            }

            $(document).on('change', ".attributeVariants", function (e) {
                $('.merge_attribute').show();
            });



            function clearAttributes() {
                $('.clear_button').click(function () {
                    $('#option_data').removeAttr('readonly');
                    $('.merge_attribute').hide();
                    $('.attribute_options').attr('style', '');
                    $('#attributeValues').html("");
                    $('#option_data').val(null).trigger('change');
                    $('.clear_button').attr('style', 'display:none');
                    $("#option_data").select2({
                        "disabled": false
                    });
                    $(".attributeVariants").select2({
                        "disabled": false
                    });
                    array = [];
                    $('#attribute_table').attr('style', 'display:none');
                    //$('#product_stock').attr('style', '');
                    $('#product_stock').removeClass('hidden');
                    $('#attribute_image_table').attr('style', 'display:none');
                    $('#tbody').html('');
                    $('#tbody_varient').html('');
                });
            }

            var navigateToFormStep = (stepNumber) => {
                document.querySelectorAll(".form-step").forEach((formStepElement) => {
                    formStepElement.classList.add("d-none");
                });
                document.querySelectorAll(".form-stepper-list").forEach((formStepHeader) => {
                    formStepHeader.classList.add("form-stepper-unfinished");
                    formStepHeader.classList.remove("form-stepper-active", "form-stepper-completed");
                });
                document.querySelector("#step-" + stepNumber).classList.remove("d-none");
                var formStepCircle = document.querySelector('li[step="' + stepNumber + '"]');
                formStepCircle.classList.remove("form-stepper-unfinished", "form-stepper-completed");
                formStepCircle.classList.add("form-stepper-active");
                for (var index = 0; index < stepNumber; index++) {
                    var formStepCircle = document.querySelector('li[step="' + index + '"]');
                    if (formStepCircle) {
                        formStepCircle.classList.remove("form-stepper-unfinished", "form-stepper-active");
                        formStepCircle.classList.add("form-stepper-completed");
                    }
                }
            }

            var checkRequiredField = (stepNumber) => {
                // return true;
                debugger;
                if (stepNumber == 2) {

                    if ($('#price').val() == null || $('#price').val() == undefined || $('#price').val() == "" || $('#price').val() == NaN) {
                        $('.priceError').text("Please enter price")
                        return false;
                    }
                    else {
                        var myPrice = $('#price').val()
                        var firstCharacter = myPrice.charAt(0);
                        if (firstCharacter == "0") {
                            $('.priceError').text("Please enter a value greater than or equal to 1.")
                            return false;
                        }
                        else {
                            $('.priceError').text()
                        }

                    }

                    var regex = /&(nbsp|amp|quot|lt|gt);/g;
                    if ($('#LongDescription').val().replace(regex, "").replace('<p>', '').replace('</p>\n', '').replace(/\s/g, "").trim() == "") {
                        $('.errLongDescription').html('Please enter product description');
                        /*returnvalue = false;*/
                        return false;
                    }
                    else {
                        $('.errLongDescription').html('');
                        /* returnvalue = true;*/
                    }
                }
                if (stepNumber == 3) {
                    var attribute = $('#option_data').val()
                    if (attribute.length != 0) {
                        for (var i = 0; i < attribute.length; i++) {
                            var Id = "#attribute_options_" + attribute[i];
                            var errorId = "#attribute_options_error_" + attribute[i];
                            if ($(Id).val() == null || $(Id).val() == undefined || $(Id).val() == '' || $(Id).val() == "" || $(Id).val().length == 0) {
                                $(errorId).text('Please select variant of selected attribute')
                                return false;
                            }
                            else {
                                $(errorId).text('')
                            }
                        }
                    }

                    if (!$('.merge_attribute').is(':hidden')) {
                        $('#error_merge_attribute').text('Please merge attribute.')
                        return false;
                    }
                    else {
                        $('#error_merge_attribute').text('')
                    }
                }
                if (stepNumber == 4) {
                    if (($('#Id').val() == '' || $('#Id').val() == "" || $('#Id').val() == undefined || $('#Id').val() == null || $('#Id').val() == 0) && ($('#MyImage').val() == '' || $('#MyImage').val() == null || $('#MyImage').val() == undefined || $('#MyImage').val() == "")) {
                        $('.errorProductImage').text('Please Upload Image')
                        return false;
                    }
                    else {
                        $('.errorProductImage').text('')
                    }
                }
                var prevStepNumber = stepNumber - 1;
                var prevStep = document.querySelector("#step-" + prevStepNumber);
                // console.log(prevStep);
                var rows = prevStep.getElementsByClassName("required");

                for (var i = 0; i < rows.length; i++) {
                    var element = rows[i].getElementsByClassName("form-control");
                    if (element[0].value === '' && i != 7) {
                        /*  alert('There are some required fields!');*/
                        return false;
                    }
                }
                return true;
            }

            document.querySelectorAll(".btn-navigate-form-step").forEach((formNavigationBtn) => {
                formNavigationBtn.addEventListener("click", () => {
                    var stepNumber = parseInt(formNavigationBtn.getAttribute("step_number"));

                    if (!formNavigationBtn.classList.contains("is_previous")) {
                        if (checkRequiredField(stepNumber)) {
                            navigateToFormStep(stepNumber);
                        }
                    } else {
                        navigateToFormStep(stepNumber);
                    }
                });
            });

            function checkNavigate(stepNumber) {
                if (checkRequiredField(stepNumber)) {
                    navigateToFormStep(stepNumber);
                }
            }

            function mergeAttributes() {
                $(".merge_attribute").on("click", function (event) {
                    debugger;
                    isMergeButtonClicked++;
                    var attribute = $('#option_data').val()
                    if (attribute.length != 0) {
                        for (var i = 0; i < attribute.length; i++) {
                            var Id = "#attribute_options_" + attribute[i];
                            var errorId = "#attribute_options_error_" + attribute[i];
                            if ($(Id).val() == null || $(Id).val() == undefined || $(Id).val() == '' || $(Id).val() == "" || $(Id).val().length == 0) {
                                $(errorId).text('Please select variant of selected attribute')
                                return false;
                            }
                            else {
                                $(errorId).text('')
                            }
                        }
                    }

                    let option = $('#option_data').val();
                    $(".merge_attribute").hide();
                    array = [];
                    var data = $('.attribute-select').select2('data')
                    option.some(function (el) {
                        let variant = [];
                        $('#attribute_options_' + el + ' option:selected').each(function () {
                            var data_title = $('#attribute_options_' + el).attr('data-title');
                            variant.push(data_title + ':' + $(this).text());
                        });
                        array.push(variant);
                    });

                    function cartesian(args) {
                        var r = [],
                            max = args.length - 1;

                        function helper(arr, i) {
                            for (var j = 0, l = args[i].length; j < l; j++) {
                                var a = arr.slice(0);

                                a.push(args[i][j]);
                                if (i == max)
                                    r.push(a);
                                else
                                    helper(a, i + 1);
                            }
                        }
                        helper([], 0);

                        return r;
                    }

                    if (array != 0) {

                        $('#tbody').html('');
                        $('#tbody_varient').html('');
                        $('#attribute_table').attr('style', '');
                        //$('#product_stock').attr('style', 'display:none');
                        $('#product_stock').addClass('hidden');
                        $('#attribute_image_table').attr('style', '');
                        var seelctedSlugValues = '[]';
                        var allimg = [];
                        allimg = JSON.parse($('#EditAttributeImage').val());

                        var list = [];
                        list = JSON.parse($('#AttributeValuseDetails').val());

                        $.each(list, function (index, value) {
                            //if (value != undefined && value != null && value.varientText != array[0][index] && array[0][index] != null && array[0][index] != undefined) {
                            //    list.splice(index,1);
                            //}
                        });

                        for (var k = 0; k <= cartesian(array).length - 1; k++) {
                            var img = []
                            var Regular_price = null;
                            var price = null;
                            var stock = null;
                            var varient_id = null;
                            if (list != null) {
                                if (list.length > 0 && list[k] != null && list[k] != undefined) {
                                    Regular_price = list[k].regularPrice;
                                    price = list[k].price;
                                    stock = list[k].stock;
                                    img = allimg.filter(a => a.productAttributeDetailId == list[k].varient_id)
                                }
                            }
                            $.each(JSON.parse(seelctedSlugValues), function (key, attr) {
                                $newslug = '';
                                if (cartesian(array)[k]) {
                                    $.each(cartesian(array)[k], function (key1, attr1) {
                                        if (key1 == 0) {
                                            $newslug = attr1.split(':')[1].toLowerCase();
                                        } else {
                                            $newslug += '_' + attr1.split(':')[1].toLowerCase();
                                        }
                                    })
                                }
                                if ($newslug == attr.attribute_slug) {
                                    Regular_price = attr.regular_price;
                                    price = attr.price;
                                    stock = attr.stock;
                                    varient_id = attr.id;
                                    console.log('attr', attr);
                                }
                            })
                            if (price != null && Regular_price != null && stock != null) {
                                var remopve_id = "'removeVarientOption" + (varient_id) + "'";
                                var remopve_image_id = "'removeVarientImage" + (varient_id) + "'";
                            } else {
                                var remopve_id = "'removeVarientOption" + (k + 1) + "'";
                                var remopve_image_id = "'removeVarientImage" + (k + 1) + "'";
                            }

                            if ($('#Id').val() > 0) {
                                if (price != null && Regular_price != null && stock != null) {
                                    $html = '<tr id="removeVarientOption' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
                                    $html += '<input type="hidden" name="attribute[' + k + '][id]" value="' + varient_id + '">';
                                    $html += '<td>' + cartesian(array)[k] + '<input type="hidden" name="attribute[' + k + '][attribute]" value="' + cartesian(array)[k] + '" ></td>';
                                    var count_function = "'" + (k + 112) + "'";
                                    $html += '<td><input type="text" class="form-control data_regularprice " data-FieldId="' + k + '"  name="RegularPriceList" id="attribute[' + k + '][regularprice]" maxlength="5" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="regulaPrice" onkeyup="" onkeypress="return  NumberOnly(event)" data-valmsg-for="email" data-valmsg-replace="true"   data-val="true" placeholder="Regular price" value="' + Regular_price + '"><span class="text-danger" id="regularPriceError' + k + '"></span></td>';
                                    $html += '<td><input type="text" class="form-control "  name="DiscountPriceList"  data-FieldId="' + k + '" id="attribute[' + k + '][discount_price]" maxlength="5" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="discountPrice" "  data-valmsg-for="email" data-valmsg-replace="true"   data-val="true"   placeholder="Discounted Price" onkeypress="return  NumberOnly(event)" value="' + price + '" > <span class="help-block text-danger"  id="discount_error' + k + '"></span></td>';
                                    $html += '<td><input type="text" class="form-control"  name="StockPriceList" id="attribute[' + k + '][stock]" maxlength="5" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="stockProduct" placeholder="Available stock" value="' + stock + '"><span class="text-danger" id="stockPriceError' + k + '"></span></td>';
                                    $html += '<td class="deleterow" id="removeVarientOption' + (k + 1) + ',removeVarientImage' + (k + 1) + '"><i class="bi bi-trash text-danger" title="Delete" data-toggle="tooltip"></i></td>';
                                    $html += '</tr>';
                                    $('#tbody').append($html);

                                    $htmlImage = '<tr class="Image_Data"  id="removeVarientImage' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
                                    $htmlImage += '<td>' + cartesian(array)[k];
                                    $htmlImage += '</td>';
                                    $htmlImage += '<td><input type="file" class="form-control btnUploadFile"  id="Image' + k + '" name="Image' + k + '" accept="image/x-png,image/gif,image/jpeg"/></td>';
                                    $htmlImage += '</tr>';
                                    if (allimg != null && allimg.length > 0) {
                                        if (img != null && img.length > 0) {
                                            $htmlImage += '<tr class="Image_Only" id="Image_remove"><td colspan="3">';
                                            for (var imgindex = 0; imgindex < img.length; imgindex++) {
                                                $htmlImage += '<div style="float: left;margin: 8px;"><img style="height:100px;width:130px;" src="/Uploads/' + img[imgindex].imageName + '" /><button class="btn btn-danger btn-block remove-item-btn remove" type="button" imgname="' + img[imgindex].imageName + '" imgid="' + img[imgindex].id + '" style="height:36px;width:130px;" title="Remove">Remove</button></div>';
                                            }
                                            $htmlImage += '</td ></tr > ';
                                        }
                                    }
                                    $('#tbody_varient').append($htmlImage);

                                }
                                else {
                                    if (isMergeButtonClicked > 2 || isMergeButtonClicked == 1) {
                                        debugger;
                                        $html = '<tr id="removeVarientOption' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
                                        $html += '<input type="hidden" name="attribute[' + k + '][id]" value="' + varient_id + '">';
                                        $html += '<td>' + cartesian(array)[k] + '<input type="hidden" name="attribute[' + k + '][attribute]" value="' + cartesian(array)[k] + '" ></td>';
                                        var count_function = "'" + (k + 112) + "'";
                                        $html += '<td><input type="text" class="form-control data_regularprice " data-FieldId="' + k + '" name="RegularPriceList" id="attribute[' + k + '][regularprice]" maxlength="5" onkeypress="return  NumberOnly(event)" oninput="this.value = this.value.replace(/[^10-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="regulaPrice" onkeyup=""     data-valmsg-for="email" data-valmsg-replace="true"  data-val="true" placeholder="Regular price"><span class="text-danger" id="regularPriceError' + k + '"></span></td>';
                                        $html += '<td><input type="text" class="form-control " data-FieldId="' + k + '"  name="DiscountPriceList" id="attribute[' + k + '][discount_price]" maxlength="5" onkeypress="return  NumberOnly(event)" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="discountPrice" "   data-valmsg-for="email" data-valmsg-replace="true"  data-val="true" placeholder="Discounted Price"> <span class="help-block text-danger"  id="discount_error' + k + '"></span></td>';
                                        $html += '<td><input type="text" class="form-control"  name="StockPriceList" id="attribute[' + k + '][stock]" maxlength="5" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="stockProduct" placeholder="Available stock"><span class="text-danger" id="stockPriceError' + k + '"></span></td>';
                                        $html += '<td class="deleterow" id="removeVarientOption' + (k + 1) + ',removeVarientImage' + (k + 1) + '"><i class="bi bi-trash text-danger" title="Delete" data-toggle="tooltip"></i></td>';
                                        $html += '</tr>';
                                        $('#tbody').append($html);

                                        $htmlImage = '<tr class="Image_Data"  id="removeVarientImage' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
                                        $htmlImage += '<td>' + cartesian(array)[k];
                                        $htmlImage += '</td>';
                                        $htmlImage += '<td><input type="file" class="form-control btnUploadFile" id="Image' + k + '" name="Image' + k + '" accept="image/x-png,image/gif,image/jpeg"/ multiple></td>';
                                        $htmlImage += '</tr>';
                                        if (allimg != null && allimg.length > 0) {
                                            if (img != null && img.length > 0) {
                                                $htmlImage += '<tr class="Image_Only" id="Image_remove"><td colspan="3">';
                                                for (var imgindex = 0; imgindex < img.length; imgindex++) {
                                                    $htmlImage += '<div style="float: left;margin: 8px;"><img style="height:100px;width:130px;" src="/Uploads/' + img[imgindex].imageName + '" /><button class="btn btn-danger btn-block remove-item-btn remove" type="button" imgname="' + img[imgindex].imageName + '" imgid="' + img[imgindex].id + '" style="height:36px;width:130px;" title="Remove">Remove</button></div>';
                                                }
                                                $htmlImage += '</td ></tr > ';
                                            }
                                        }
                                        $('#tbody_varient').append($htmlImage);
                                    }
                                }
                            }
                            else {
                                $html = '<tr id="removeVarientOption' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
                                $html += '<input type="hidden" name="attribute[' + k + '][id]" value="' + varient_id + '">';
                                $html += '<td>' + cartesian(array)[k] + '<input type="hidden" name="attribute[' + k + '][attribute]" value="' + cartesian(array)[k] + '" ></td>';
                                var count_function = "'" + (k + 112) + "'";
                                $html += '<td><input type="text" class="form-control data_regularprice " data-FieldId="' + k + '" name="RegularPriceList" id="attribute[' + k + '][regularprice]" maxlength="5" onkeypress="return  NumberOnly(event)" oninput="this.value = this.value.replace(/[^10-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="regulaPrice" onkeyup=""     data-valmsg-for="email" data-valmsg-replace="true"  data-val="true" placeholder="Regular price"><span class="text-danger" id="regularPriceError' + k + '"></span></td>';
                                $html += '<td><input type="text" class="form-control " data-FieldId="' + k + '"  name="DiscountPriceList" id="attribute[' + k + '][discount_price]" maxlength="5" onkeypress="return  NumberOnly(event)" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="discountPrice" "   data-valmsg-for="email" data-valmsg-replace="true"  data-val="true" placeholder="Discounted Price"> <span class="help-block text-danger"  id="discount_error' + k + '"></span></td>';
                                $html += '<td><input type="text" class="form-control"  name="StockPriceList" id="attribute[' + k + '][stock]" maxlength="5" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="stockProduct" placeholder="Available stock"><span class="text-danger" id="stockPriceError' + k + '"></span></td>';
                                $html += '<td class="deleterow" id="removeVarientOption' + (k + 1) + ',removeVarientImage' + (k + 1) + '"><i class="bi bi-trash text-danger" title="Delete" data-toggle="tooltip"></i></td>';
                                $html += '</tr>';
                                $('#tbody').append($html);

                                $htmlImage = '<tr class="Image_Data"  id="removeVarientImage' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
                                $htmlImage += '<td>' + cartesian(array)[k];
                                $htmlImage += '</td>';
                                $htmlImage += '<td><input type="file" class="form-control btnUploadFile" id="Image' + k + '" name="Image' + k + '" accept="image/x-png,image/gif,image/jpeg"/ multiple></td>';
                                $htmlImage += '</tr>';
                                if (allimg != null && allimg.length > 0) {
                                    if (img != null && img.length > 0) {
                                        $htmlImage += '<tr class="Image_Only" id="Image_remove"><td colspan="3">';
                                        for (var imgindex = 0; imgindex < img.length; imgindex++) {
                                            $htmlImage += '<div style="float: left;margin: 8px;"><img style="height:100px;width:130px;" src="/Uploads/' + img[imgindex].imageName + '" /><button class="btn btn-danger btn-block remove-item-btn remove" type="button" imgname="' + img[imgindex].imageName + '" imgid="' + img[imgindex].id + '" style="height:36px;width:130px;" title="Remove">Remove</button></div>';
                                        }
                                        $htmlImage += '</td ></tr > ';
                                    }
                                }
                                $('#tbody_varient').append($htmlImage);
                            }
                        }
                    }
                });
            }


            //function mergeAttributes() {
            //    $(".merge_attribute").bind("click", function () {

            //        let option = $('#option_data').val();
            //        $(".merge_attribute").hide();
            //        array = [];
            //        var data = $('.attribute-select').select2('data')
            //        option.some(function (el) {
            //            let variant = [];
            //            $('#attribute_options_' + el + ' option:selected').each(function () {
            //                var data_title = $('#attribute_options_' + el).attr('data-title');
            //                variant.push(data_title + ':' + $(this).text());
            //            });
            //            array.push(variant);
            //        });

            //        function cartesian(args) {
            //            var r = [],
            //                max = args.length - 1;

            //            function helper(arr, i) {
            //                for (var j = 0, l = args[i].length; j < l; j++) {
            //                    var a = arr.slice(0);

            //                    a.push(args[i][j]);
            //                    if (i == max)
            //                        r.push(a);
            //                    else
            //                        helper(a, i + 1);
            //                }
            //            }
            //            helper([], 0);

            //            return r;
            //        }

            //        if (array != 0) {

            //            $('#tbody').html('');
            //            $('#tbody_varient').html('');
            //            $('#attribute_table').attr('style', '');
            //            $('#product_stock').attr('style', 'display:none');
            //            $('#attribute_image_table').attr('style', '');
            //            var seelctedSlugValues = '[]';
            //            var allimg = [];
            //            allimg = JSON.parse($('#EditAttributeImage').val());

            //            var list = [];
            //            list = JSON.parse($('#AttributeValuseDetails').val());

            //            for (var k = 0; k <= cartesian(array).length - 1; k++) {
            //                var img = []
            //                var Regular_price = null;
            //                var price = null;
            //                var stock = null;
            //                var varient_id = null;
            //                if (list != null) {
            //                    Regular_price = list[k].regularPrice;
            //                    price = list[k].price;
            //                    stock = list[k].stock;
            //                    img = allimg.filter(a => a.productAttributeDetailId == list[k].varient_id)
            //                }
            //                $.each(JSON.parse(seelctedSlugValues), function (key, attr) {
            //                    $newslug = '';
            //                    if (cartesian(array)[k]) {
            //                        $.each(cartesian(array)[k], function (key1, attr1) {
            //                            if (key1 == 0) {
            //                                $newslug = attr1.split(':')[1].toLowerCase();
            //                            } else {
            //                                $newslug += '_' + attr1.split(':')[1].toLowerCase();
            //                            }
            //                        })
            //                    }
            //                    if ($newslug == attr.attribute_slug) {
            //                        Regular_price = attr.regular_price;
            //                        price = attr.price;
            //                        stock = attr.stock;
            //                        varient_id = attr.id;
            //                        console.log('attr', attr);
            //                    }
            //                })
            //                if (price != null && Regular_price != null && stock != null) {
            //                    var remopve_id = "'removeVarientOption" + (varient_id) + "'";
            //                    var remopve_image_id = "'removeVarientImage" + (varient_id) + "'";
            //                } else {
            //                    var remopve_id = "'removeVarientOption" + (k + 1) + "'";
            //                    var remopve_image_id = "'removeVarientImage" + (k + 1) + "'";
            //                }

            //                $html = '<tr id="removeVarientOption' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
            //                $html += '<input type="hidden" name="attribute[' + k + '][id]" value="' + varient_id + '">';
            //                $html += '<td>' + cartesian(array)[k] + '<input type="hidden" name="attribute[' + k + '][attribute]" value="' + cartesian(array)[k] + '" ></td>';
            //                var count_function = "'" + (k + 112) + "'";
            //                /*"discount_error' + (k + 112) + '"*/
            //                if (Regular_price != null && price != null && stock != null) {
            //                    $html += '<td><input type="text" class="form-control data_regularprice check_price" data-FieldId="' + k + '"  name="RegularPriceList" id="attribute[' + k + '][regularprice]" maxlength="10" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="regulaPrice" onkeyup="check_price(' + Regular_price + ')" required placeholder="Regular price" value="' + Regular_price + '"></td>';
            //                } else {
            //                    $html += '<td><input type="text" class="form-control data_regularprice check_price" data-FieldId="' + k + '" name="RegularPriceList" id="attribute[' + k + '][regularprice]" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="regulaPrice" onkeyup="check_price(' + Regular_price + ')"    required placeholder="Regular price"></td>';
            //                }

            //                if (price != null && Regular_price != null && stock != null) {
            //                    $html += '<td><input type="text" class="form-control check_price"  name="DiscountPriceList"  data-FieldId="' + k + '" id="attribute[' + k + '][discount_price]" maxlength="10" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="discountPrice" onkeyup="check_price(' + (k + 112) + ')" required   placeholder="Discounted Price" value="' + price + '" ><br/> <span class="help-block"  id="discount_error' + (k + 112) + '" style="display: none;"></span></td>';
            //                } else {
            //                    $html += '<td><input type="text" class="form-control check_price" data-FieldId="' + k + '"  name="DiscountPriceList" id="attribute[' + k + '][discount_price]" maxlength="10" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="discountPrice" onkeyup="check_price(' + (k + 112) + ')" required  placeholder="Discounted Price"><br/> <span class="help-block"  id="discount_error' + (k + 112) + '" style="display: none;"></span></td>';
            //                }
            //                if (stock != null && price != null && Regular_price != null) {
            //                    $html += '<td><input type="text" class="form-control"  name="StockPriceList" id="attribute[' + k + '][stock]" maxlength="10" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="stockProduct" required placeholder="Available stock" value="' + stock + '"></td>';
            //                } else {
            //                    $html += '<td><input type="text" class="form-control"  name="StockPriceList" id="attribute[' + k + '][stock]" maxlength="10" oninput="this.value = this.value.replace(/[^0-9.]/g, \'\').replace(/(\\..*)\\./g, \'$1\');" style="width:150px;" class="stockProduct" required  placeholder="Available stock"></td>';
            //                }
            //                if (price != null && Regular_price != null && stock != null) {
            //                    $html += '<td class="deleterow"><i class="bi bi-trash text-danger" title="Delete" data-toggle="tooltip" onclick="removeVarientAjax(' + remopve_id + ')"></i></td>';
            //                } else {
            //                    $html += '<td><i style="width:150px;" class="bi bi-trash text-danger" title="Delete" data-toggle="tooltip" onclick="removeVarient(' + remopve_id + ');removeVarientIm(' + remopve_image_id + ');"></i></td>';
            //                }
            //                $html += '</tr>';
            //                $('#tbody').append($html);

            //                $htmlImage = '<tr class="Image_Data"  id="removeVarientImage' + (k + 1) + '"><th scope="row_' + (k + 1) + '">' + (k + 1) + '</th>';
            //                $htmlImage += '<td>' + cartesian(array)[k];


            //                $htmlImage += '</td>';

            //                if (stock != null) {
            //                    $htmlImage += '<td><input type="file" class="form-control btnUploadFile"  id="Image' + k + '" name="Image' + k + '" accept="image/x-png,image/gif,image/jpeg"/></td>';

            //                } else {
            //                    $htmlImage += '<td><input type="file" class="form-control btnUploadFile" id="Image' + k + '" name="Image' + k + '" accept="image/x-png,image/gif,image/jpeg"/ multiple></td>';
            //                }

            //                $htmlImage += '</tr>';
            //                if (allimg != null && allimg.length > 0) {
            //                    if (img != null && img.length > 0) {
            //                        $htmlImage += '<tr class="Image_Only" id="Image_remove"><td colspan="3">';
            //                        for (var imgindex = 0; imgindex < img.length; imgindex++) {
            //                            $htmlImage += '<div style="float: left;margin: 8px;"><img style="height:100px;width:130px;" src="/Uploads/' + img[imgindex].imageName + '" /><button class="btn btn-danger btn-block remove-item-btn remove" type="button" imgname="' + img[imgindex].imageName + '" imgid="' + img[imgindex].id + '" style="height:36px;width:130px;" title="Remove">Remove</button></div>';
            //                        }
            //                        $htmlImage += '</td ></tr > ';
            //                    }
            //                }


            //                $('#tbody_varient').append($htmlImage);
            //            }
            //        }
            //    });
            //}

            $(document).on('change', '.option_value_select', function (e) {

                var option_hex = $(this).find(':selected').attr('data-option_hex');
                var option_value_id = $(this).find(':selected').attr('data-option_value_id');
                var option_row_id = $(this).find(':selected').attr('data-id');
                $('input[name="input[' + option_row_id + '][option_hex]"]').val(option_hex);
                $('input[name="input[' + option_row_id + '][option_value_id]"]').val(option_value_id);
            })

            $(document).on("click", '.deleterow', function () {
                debugger;
                var varientId = $(this).attr('id');
                if (varientId.length > 0) {
                    $('#' + varientId.split(',')[0]).remove();
                    $('#' + varientId.split(',')[1]).remove();
                }
                //var imgdiv = $(this).parent();
                //imgdiv.remove();
            });
            $(document).on("change", '.check_price', function (event) {

                var price = $(this).val();
                var prev = $(this).parent().prev().find('.check_price').val();
                var next = $(this).parent().Next().find('.check_price').val();
                var index = $(this).attr('data-FieldId');
                var pID = "attribute[" + index + "][regularprice]";
                var dID = "attribute[" + index + "][discount_price]";
                var price = $("#attribute[0][regularprice]").val();
                var discounted_price = $('#' + dID).val();
                if (price == "") {
                    $("#attribute['" + index + "'][discount_price]").val('');
                    $("#attribute['" + index + "'][discount_price]").html("Please enter the regular price first");
                    $("#attribute['" + index + "'][discount_price]").show();
                } else {
                    $("#attribute['" + index + "'][discount_price]").hide();
                    if (price <= discounted_price) {
                        $("#attribute['" + index + "'][discount_price]").val('');
                        $("#attribute['" + index + "'][discount_price]").html("Discount price not more than regular price");
                        $("#attribute['" + index + "'][discount_price]").show();
                    }
                }
            });


            $(document).on("click", '.final-submit', function () {
                debugger;
                if ($('#attribute_table table tbody input[type="text"]').length > 0 && $('#attribute_table table tbody input[type="text"]').length != null) {

                }
            });


            function removeVarient(el) {

                //$("#" + el.split(',')[0]).remove();
                //$("#" + el.split(',')[1]).remove();
            }
            function removeVarientIm(el) {
                $("#" + el).remove();
            }

            $(document).on("click", '.Image_Only button', function () {

                var imgdiv = $(this).parent();
                var Image_remove_stroe = {};
                Image_remove_stroe.Id = $(this).attr('imgid');
                Image_remove_stroe.AttributeImages = $(this).attr('imgname');
                arrImage_remove_store.push(Image_remove_stroe)
                imgdiv.remove();
            });

            function ckEditor() {
                CKEDITOR.replace('LongDescription', { extraAllowedContent: '*(*);*{*}' });
                CKEDITOR.instances['LongDescription'].on("instanceReady", function () {
                    this.on("change", function () {
                        CKEDITOR.instances['LongDescription'].updateElement();
                    });
                });
            }
        }

        function InitForm() {
            var optionData = $('#optionData').val();

            if (optionData != null && optionData != '' && optionData.split(',').length > 0) {
                var dataOption = optionData.split(',');

                $('#option_data').val(dataOption).change();
                $('.variant_button').trigger('click');
                check_price();
            }
        }

        $this.init = function () {
            initializeGrid();
            InitForm();

            $formAddEditproduct = new Global.FormHelperWithFiles($(this).find("form"),
                { updateTargetId: "validation-summary" }, function onSuccess(result) {
                    location.href = '/Admin/Products/Index';
                });

            $("body").on("click", ".final-submit", function () {

                if (!$('#product_stock').hasClass('hidden')) {
                    if ($('#AvailableStock').val() == "" || $('#AvailableStock').val() == '' && $('#AvailableStock').val() == null || $('#AvailableStock').val() == undefined || $('#AvailableStock').val() == 0) {
                        $('.errorAvailableStock').text('Available stock required.')
                        $('.errorAvailableStock').addClass('text-danger')
                        return false;
                    }
                    else {
                        $('.errorAvailableStock').text('')
                    }
                    //if ($('#StockClose').val() == "" || $('#StockClose').val() == '' && $('#StockClose').val() == null || $('#StockClose').val() == undefined || $('#StockClose').val() == 0) {
                    //    $('.errorStockClose').text('Closing stock required.')
                    //    $('.errorStockClose').addClass('text-danger')
                    //    return false;
                    //}
                    //else {
                    //    if (parseInt($('#AvailableStock').val()) < parseInt($('#StockClose').val())) {
                    //        $('.errorStockClose').text('Closing stock should be less than available stock.')
                    //        $('.errorStockClose').addClass('text-danger')
                    //        return false;
                    //    }
                    //    else {
                    //        $('.errorStockClose').text('')
                    //    }
                    //    $('.errorStockClose').text('')
                    //}
                }


                var customers = new Array();
                $("#tbody_varient .Image_Data").each(function () {
                    var row = $(this);
                    var customer = {};
                    customer.Name = row.find("TD").eq(0).html();
                    /*customer.Files = row.find("TD").eq(1).find('input[type=file]').get(0);*/

                    var fileU = (row.find("td").eq(1).find('input[type=file]')).get(0).files
                    for (var i = 0; i < fileU.length; i++) {
                        customer.VarientImageName = fileU[i].name;
                    }
                    customers.push(customer);
                });
                var data = JSON.stringify(customers);
                $('#AdditionalRowsJSON').val(data);

                var Remove_attrebute_image = JSON.stringify(arrImage_remove_store);
                $('#hdnAttribute_RemoveImage').val(Remove_attrebute_image);


                var formData = new FormData();

                var form_data = $('#frmproductmanager').serializeArray();
                $.each(form_data, function (key, input) {
                    formData.append(input.name, input.value);
                });

                // single image
                Length = $('#MyImage').length
                for (var i = 0; i < Length; i++) {
                    var files = $('#MyImage').get(i).files[0];
                    formData.append('MyImage', files);
                }

                // multiple image
                Length = $('.btnUploadFile').length
                for (var i = 0; i < Length; i++) {
                    var files = $('.btnUploadFile').get(i).files[0];
                    formData.append('hdnMultipleImage', files);
                }
                //Available Quantity for Varient
                var RegularPriceList = $('input[name="RegularPriceList"]').map(function () {
                    return this.value;
                }).get();
                var StockPriceList = $('input[name="StockPriceList"]').map(function () {
                    return this.value;
                }).get();
                var DiscountPriceList = $('input[name="DiscountPriceList"]').map(function () {
                    return this.value;
                }).get();
                // check Merge Value aviable or not  if avail then check regular price list not null
                let option = $('#option_data').val();
                $(".merge_attribute").hide();
                array = [];
                var data = $('.attribute-select').select2('data')
                option.some(function (el) {
                    let variant = [];
                    $('#attribute_options_' + el + ' option:selected').each(function () {
                        var data_title = $('#attribute_options_' + el).attr('data-title');
                        variant.push(data_title + ':' + $(this).text());
                    });
                    array.push(variant);
                    //alert(variant);
                });

                if (array.length > 0) {
                    if (RegularPriceList.length == 0 || RegularPriceList.length == undefined) {

                        //alert('Please enter all details !');
                        return false;
                    }
                }
                if (RegularPriceList.length > 0) {
                    debugger;
                    var returnValue = true;
                    for (var i = 0; i < RegularPriceList.length; i++) {
                        if (StockPriceList[i] == "" || StockPriceList[i] == undefined || StockPriceList[i] == null || StockPriceList[i] == "0" || RegularPriceList[i] == "" || RegularPriceList[i] == undefined || RegularPriceList[i] == null || RegularPriceList[i] == "0") {
                            if (StockPriceList[i] == "" || StockPriceList[i] == undefined || StockPriceList[i] == null) {
                                $('#stockPriceError' + i).text('Stock required.')
                                returnValue = false;
                            }
                            else {
                                if (StockPriceList[i] == "0") {
                                    $('#stockPriceError' + i).text('Stock should not be 0')
                                    returnValue = false;
                                }
                                else {
                                    $('#stockPriceError' + i).text('')
                                }
                            }
                            if (RegularPriceList[i] == "" || RegularPriceList[i] == undefined || RegularPriceList[i] == null) {
                                $('#regularPriceError' + i).text('Regular price required.')
                                returnValue = false;
                            }
                            else {
                                if (RegularPriceList[i] == "0") {
                                    $('#regularPriceError' + i).text('Regular price should not be 0')
                                    returnValue = false;
                                }
                                else {
                                    $('#regularPriceError' + i).text('')
                                }
                            }
                        }
                        else {
                            $('#stockPriceError' + i).text('')
                            $('#regularPriceError' + i).text('')
                            if (parseFloat(RegularPriceList[i]) <= parseFloat(DiscountPriceList[i])) {
                                RegularPriceList[i] = "";
                                $('#discount_error' + i).html('Discount price should not be <br> greater than regular price.')
                                returnValue = false;
                            }
                            else {
                                $('#discount_error' + i).html('')
                            }
                        }

                    }
                    if (!returnValue) {
                        return false;
                    }

                }


                if ($("form").valid()) {
                    debugger;
                    $.ajax({
                        url: '/Products/Create',
                        type: 'post',
                        data: formData,
                        contentType: false,
                        processData: false,
                        success: function (response) {

                            if (response.isSuccess == true) {
                                location.href = '/Admin/Products/Index';
                                //window.location.href = response.redirectuUrl;
                            }
                        },
                    });
                }
            });

           

            //$(document).off('keypress', '#price').on('keypress', '#price', function (e) {
            //    // 46 is the key code of the dot
            //    if (e.keyCode == 46) {
            //        //$('#price').val('')
            //        return false;
            //    }
            //}); 

            //$("body").on("click", ".final-submit", function () {

            //    var customers = new Array();
            //    $("#tbody_varient .Image_Data").each(function () {
            //        var row = $(this);
            //        var customer = {};
            //        customer.Name = row.find("TD").eq(0).html();
            //        /*customer.Files = row.find("TD").eq(1).find('input[type=file]').get(0);*/

            //        var fileU = (row.find("td").eq(1).find('input[type=file]')).get(0).files
            //        for (var i = 0; i < fileU.length; i++) {
            //            customer.VarientImageName = fileU[i].name;
            //        }
            //        customers.push(customer);
            //    });
            //    var data = JSON.stringify(customers);
            //    $('#AdditionalRowsJSON').val(data);

            //    var Remove_attrebute_image = JSON.stringify(arrImage_remove_store);
            //    $('#hdnAttribute_RemoveImage').val(Remove_attrebute_image);


            //    var formData = new FormData();

            //    var form_data = $('#frmproductmanager').serializeArray();
            //    $.each(form_data, function (key, input) {
            //        formData.append(input.name, input.value);
            //    });

            //    // single image
            //    Length = $('#MyImage').length
            //    for (var i = 0; i < Length; i++) {
            //        var files = $('#MyImage').get(i).files[0];
            //        formData.append('MyImage', files);
            //    }

            //    // multiple image
            //    Length = $('.btnUploadFile').length
            //    for (var i = 0; i < Length; i++) {
            //        var files = $('.btnUploadFile').get(i).files[0];
            //        formData.append('hdnMultipleImage', files);
            //    }

            //    if ($("form").valid()) {
            //        $.ajax({
            //            url: '/Products/Create',
            //            type: 'post',
            //            data: formData,
            //            contentType: false,
            //            processData: false,
            //            success: function (response) {

            //                if (response.isSuccess==true)
            //                {
            //                    location.href = '/Admin/Products/Index';
            //                    //window.location.href = response.redirectuUrl;
            //                }
            //                //if (response != 0) {
            //                //    var status = $('#hdnStatus').val();
            //                //    if (status == 0) {
            //                //        window.location.href = response.redirectuUrl;

            //                //    } else {
            //                //        window.location.href = response.redirectuUrl;

            //                //    }
            //                //    location.href = '/Admin/Products/Index';
            //                //}
            //                //else {
            //                //    alert('file not uploaded');
            //                //}
            //            },
            //        });
            //    }
            //});
        }

       
    }

    $(function () {
        var self = new Cms();
        self.init();
    });
})(jQuery)

