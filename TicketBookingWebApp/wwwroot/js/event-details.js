document.addEventListener("DOMContentLoaded", function () {
    const selectedSeatIds = new Set();
    const seatButtons = document.querySelectorAll(".seat-btn");
    const submitBtn = document.getElementById("submitBooking");

    const selectedSeatInput = document.getElementById("SelectedSeatIds");
    const eventId = document.getElementById("EventId").value;
    const eventTitle = document.getElementById("EventTitle").value;
    const eventDateTime = document.getElementById("EventDateTime").value;
    const isSeatBased = document.getElementById("IsSeatBased").value === "true";

    const quantityInput = document.getElementById("TicketQuantity");
    const quantityHiddenInput = document.getElementById("Quantity");
    if (quantityInput && quantityHiddenInput) {
        quantityHiddenInput.value = quantityInput.value;

        document.getElementById("increaseQty").addEventListener("click", () => {
            let value = parseInt(quantityInput.value) || 1;
            value++;
            quantityInput.value = value;
            quantityHiddenInput.value = value;
        });

        document.getElementById("decreaseQty").addEventListener("click", () => {
            let value = parseInt(quantityInput.value) || 1;
            if (value > 1) value--;
            quantityInput.value = value;
            quantityHiddenInput.value = value;
        });
    }

    seatButtons.forEach(btn => {
        btn.addEventListener("click", function () {
            const seatId = btn.getAttribute("data-seat-id");

            if (btn.disabled) return;

            if (selectedSeatIds.has(seatId)) {
                selectedSeatIds.delete(seatId);
                btn.classList.remove("btn-primary");
                btn.classList.add("btn-outline-success");
            } else {
                selectedSeatIds.add(seatId);
                btn.classList.remove("btn-outline-success");
                btn.classList.add("btn-primary");
            }

            selectedSeatInput.value = Array.from(selectedSeatIds).join(",");
        });
    });

    submitBtn.addEventListener("click", async function () {
        const confirmed = window.confirm("Are you sure you want to book these tickets?");
        if (!confirmed) return;

        const seatIdList = Array.from(selectedSeatIds).map(id => parseInt(id));
        const quantity = parseInt(quantityHiddenInput?.value || "1");

        const bookingDto = {
            eventId: parseInt(eventId),
            seatIds: seatIdList,
            eventTitle: eventTitle,
            eventDateTime: eventDateTime,
            quantity: quantity,
            isSeatBased: isSeatBased
        };

        try {
            const response = await fetch('/Event/BookTicket', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(bookingDto)
            });

            if (response.ok) {
                const data = await response.json();
                if (data.bookingId) {
                    window.location.href = `/Event/BookingDetails?bookingId=${data.bookingId}`;
                } else {
                    window.location.href = "/Event/MyBookings";
                }
            } else {
                const err = await response.text();
                alert("Booking failed: " + err);
            }
        } catch (error) {
            console.error(error);
            alert("Error booking seats.");
        }
    });
});