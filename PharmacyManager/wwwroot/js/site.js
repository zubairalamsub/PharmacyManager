
$(document).ready(function () {

    // Only run billing logic on the create invoice page
    if ($('#itemsTable').length === 0) return;

    var rowIndex = 0;

    // medicine dropdown 
    function getMedicineOptions() {
        var opts = '<option value="">-- Select Medicine --</option>';
        if (typeof medicinesData !== 'undefined') {
            for (var i = 0; i < medicinesData.length; i++) {
                var m = medicinesData[i];
                opts += '<option value="' + m.id + '">' + m.name + ' (Stock: ' + m.stock + ')</option>';
            }
        }
        return opts;
    }

    // Add new row
    $('#btnAddRow').on('click', function () {
        rowIndex++;
        var html = '<tr data-row="' + rowIndex + '">' +
            '<td><select class="med-select" data-row="' + rowIndex + '">' + getMedicineOptions() + '</select></td>' +
            '<td><input type="text" class="batch-input" data-row="' + rowIndex + '" readonly /></td>' +
            '<td><input type="date" class="expiry-input" data-row="' + rowIndex + '" readonly /></td>' +
            '<td><input type="number" class="qty-input" data-row="' + rowIndex + '" min="1" value="1" /></td>' +
            '<td><input type="number" class="price-input" data-row="' + rowIndex + '" step="0.01" readonly /></td>' +
            '<td><input type="text" class="line-total" data-row="' + rowIndex + '" readonly value="0.00" /></td>' +
            '<td><button type="button" class="btn btn-danger btn-remove" data-row="' + rowIndex + '">Remove</button></td>' +
            '</tr>';
        $('#itemsBody').append(html);
    });


    $(document).on('change', '.med-select', function () {
        var r = $(this).data('row');
        var medId = parseInt($(this).val());
        var row = $('tr[data-row="' + r + '"]');

        if (!medId) {
            row.find('.batch-input').val('');
            row.find('.expiry-input').val('');
            row.find('.price-input').val('');
            row.find('.line-total').val('0.00');
            recalcTotals();
            return;
        }

        // Find medicine in local data
        var med = null;
        for (var i = 0; i < medicinesData.length; i++) {
            if (medicinesData[i].id === medId) {
                med = medicinesData[i];
                break;
            }
        }

        if (med) {
            row.find('.batch-input').val(med.batch);
            row.find('.expiry-input').val(med.expiry);
            row.find('.price-input').val(med.price.toFixed(2));
            calcLineTotal(r);
        }
    });

    // recalculate total
    $(document).on('input', '.qty-input', function () {
        var r = $(this).data('row');
        calcLineTotal(r);
    });

    // Remove row
    $(document).on('click', '.btn-remove', function () {
        var r = $(this).data('row');
        $('tr[data-row="' + r + '"]').remove();
        recalcTotals();
    });

    //recalculate 
    $('#discountInput').on('input', function () {
        recalcTotals();
    });

    // Calculate line total for a specific row
    function calcLineTotal(r) {
        var row = $('tr[data-row="' + r + '"]');
        var qty = parseInt(row.find('.qty-input').val()) || 0;
        var price = parseFloat(row.find('.price-input').val()) || 0;
        var lineTotal = qty * price;
        row.find('.line-total').val(lineTotal.toFixed(2));
        recalcTotals();

        // Check stock availability
        var medId = parseInt(row.find('.med-select').val());
        if (medId && qty > 0) {
            checkStock(medId, qty);
        }
    }

    // Recalculate subtotal and grand total
    function recalcTotals() {
        var sub = 0;
        $('.line-total').each(function () {
            sub += parseFloat($(this).val()) || 0;
        });
        var discount = parseFloat($('#discountInput').val()) || 0;
        var grand = sub - discount;
        if (grand < 0) grand = 0;

        $('#subTotal').text(sub.toFixed(2));
        $('#grandTotal').text(grand.toFixed(2));
    }

    // Check stock availability via AJAX
    function checkStock(medicineId, quantity) {
        $.getJSON('/Billing/CheckStock', { medicineId: medicineId, quantity: quantity }, function (data) {
            var msgEl = $('#stockMessage');
            if (!data.available) {
                msgEl.text(data.message).removeClass('ok').addClass('error');
            } else {
                msgEl.text('').removeClass('error ok');
            }
        });
    }

    // invoice save
    $('#btnSaveInvoice').on('click', function () {
        var customerName = $('#customerName').val().trim();
        if (!customerName) {
            alert('Please enter the customer name.');
            $('#customerName').focus();
            return;
        }

        var rows = $('#itemsBody tr');
        if (rows.length === 0) {
            alert('Please add at least one item to the invoice.');
            return;
        }

        var items = [];
        var hasError = false;

        rows.each(function () {
            var medId = parseInt($(this).find('.med-select').val());
            if (!medId) {
                alert('Please select a medicine for all rows.');
                hasError = true;
                return false;
            }

            var qty = parseInt($(this).find('.qty-input').val()) || 0;
            if (qty <= 0) {
                alert('Quantity must be at least 1 for all items.');
                hasError = true;
                return false;
            }

            items.push({
                medicineId: medId,
                batchNumber: $(this).find('.batch-input').val(),
                expiryDate: $(this).find('.expiry-input').val(),
                quantity: qty,
                unitPrice: parseFloat($(this).find('.price-input').val()) || 0,
                lineTotal: parseFloat($(this).find('.line-total').val()) || 0
            });
        });

        if (hasError) return;

        var payload = {
            invoiceNumber: $('#invoiceNumber').val(),
            invoiceDate: $('#invoiceDate').val(),
            customerName: customerName,
            contactNumber: $('#contactNumber').val(),
            discount: parseFloat($('#discountInput').val()) || 0,
            items: items
        };

        var btn = $('#btnSaveInvoice');
        btn.prop('disabled', true).text('Saving...');

        $.ajax({
            url: '/Billing/Create',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (resp) {
                if (resp.success) {
                    alert(resp.message);
                    window.location.href = '/Billing/Details/' + resp.invoiceId;
                } else {
                    alert('Error: ' + resp.message);
                    btn.prop('disabled', false).text('Save Invoice');
                }
            },
            error: function () {
                alert('Something went wrong while saving the invoice.');
                btn.prop('disabled', false).text('Save Invoice');
            }
        });
    });

});
