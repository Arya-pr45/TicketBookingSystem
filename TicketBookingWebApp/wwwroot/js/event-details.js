document.addEventListener("DOMContentLoaded", function () {
    const selectedSeatIds = new Set();
    const seatButtons = document.querySelectorAll(".seat-btn");
    const submitBtn = document.getElementById("submitBooking");

    const eventId = document.getElementById("EventId").value;
    const eventTitle = document.getElementById("EventTitle").value;
    const eventDateTime = document.getElementById("EventDateTime").value;
    const isSeatBased = document.getElementById("IsSeatBased").value === "true";
    const price = parseFloat(document.getElementById("Price").value);
    const availableTickets = parseInt(document.getElementById("AvailableTickets").value);

    const selectedSeatInput = document.getElementById("SelectedSeatIds");
    const quantityInput = document.getElementById("TicketQuantity");
    const quantityHiddenInput = document.getElementById("Quantity");

    const limitAlert = document.getElementById("limitAlert");

    if (!isSeatBased) {
        document.getElementById("increaseQty").addEventListener("click", () => {
            let value = parseInt(quantityInput.value) || 0;
            if (value < availableTickets) {
                value++;
                quantityInput.value = value;
                quantityHiddenInput.value = value;
                limitAlert.classList.add("d-none");
            } else {
                limitAlert.classList.remove("d-none");
            }
        });

        document.getElementById("decreaseQty").addEventListener("click", () => {
            let value = parseInt(quantityInput.value) || 0;
            if (value > 0) {
                value--;
                quantityInput.value = value;
                quantityHiddenInput.value = value;
                limitAlert.classList.add("d-none");
            }
        });
    }

    seatButtons.forEach(btn => {
        btn.addEventListener("click", function () {
            const seatId = btn.getAttribute("data-seat-id");
            if (btn.classList.contains("disabled")) return;

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

    submitBtn.addEventListener("click", function () {
        const quantity = isSeatBased ? selectedSeatIds.size : parseInt(quantityHiddenInput.value || "0");
        if (quantity === 0) {
            toastr.warning("Please select at least 1 ticket.");
            return;
        }

        // Show payment modal
        document.getElementById("modalTitle").textContent = eventTitle;
        document.getElementById("modalDate").textContent = new Date(eventDateTime).toLocaleString();
        document.getElementById("modalQty").textContent = quantity;
        document.getElementById("modalTotal").textContent = quantity * price;

        new bootstrap.Modal(document.getElementById("paymentModal")).show();
    });

    document.getElementById("confirmBooking").addEventListener("click", async () => {
        const bookingDto = {
            eventId: parseInt(eventId),
            seatIds: Array.from(selectedSeatIds).map(id => parseInt(id)),
            eventTitle,
            eventDateTime,
            quantity: isSeatBased ? selectedSeatIds.size : parseInt(quantityHiddenInput.value || "0"),
            isSeatBased
        };

        try {
            const response = await fetch('/Event/BookTicket', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(bookingDto)
            });

            if (response.ok) {
                const data = await response.json();
                toastr.success("Booking successful!");

                setTimeout(() => {
                    if (data.bookingId) {
                        window.location.href = `/Event/BookingDetails?bookingId=${data.bookingId}`;
                    } else {
                        window.location.href = "/Event/MyBookings";
                    }
                }, 1000);
            } else {
                const err = await response.text();
                toastr.error("Booking failed: " + err);
            }
        } catch (error) {
            console.error(error);
            toastr.error("An error occurred while booking.");
        }
    });

    // Toggle Seat Map
    const toggleBtn = document.getElementById("toggleSeatMap");
    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            const seatBox = document.getElementById("seatMapBox");
            seatBox.classList.toggle("d-none");
        });
    }
});
