import json
import logging
import os

import azure.functions as func
from azure.communication.email import EmailClient

app = func.FunctionApp()

_EMAIL_TEMPLATES: dict[str, tuple[str, str]] = {
    "LoanApplicationApproved": (
        "Your loan application has been approved",
        "<p>Congratulations! Your loan application has been <strong>approved</strong>.</p>",
    ),
    "LoanApplicationRejected": (
        "Your loan application has been rejected",
        "<p>We regret to inform you that your loan application has been <strong>rejected</strong>.</p>",
    ),
    "FundClaimApproved": (
        "Your fund claim has been approved",
        "<p>Congratulations! Your fund claim has been <strong>approved</strong>.</p>",
    ),
    "FundClaimRejected": (
        "Your fund claim has been rejected",
        "<p>We regret to inform you that your fund claim has been <strong>rejected</strong>.</p>",
    ),
}


def _send_email(recipient_address: str, subject: str, html_body: str) -> None:
    connection_string = os.environ["COMMUNICATION_CONNECTION"]
    sender_address = os.environ["SENDER_ADDRESS"]

    client = EmailClient.from_connection_string(connection_string)
    message = {
        "senderAddress": sender_address,
        "recipients": {"to": [{"address": recipient_address}]},
        "content": {"subject": subject, "html": html_body},
    }

    poller = client.begin_send(message)
    result = poller.result()
    logging.info("Email sent. messageId=%s", result.get("id"))


@app.service_bus_queue_trigger(
    arg_name="message",
    queue_name="stafffinance-notifications",
    connection="SERVICEBUS_CONNECTION")
def notification_dispatcher(message: func.ServiceBusMessage) -> None:
    payload = json.loads(message.get_body().decode("utf-8"))
    event_type: str = getattr(message, "subject", "") or payload.get("eventType", "unknown")

    logging.info("Received notification event. eventType=%s", event_type)

    recipient: str | None = payload.get("recipientEmail")
    if not recipient:
        logging.warning("No recipientEmail in payload — skipping email dispatch. payload=%s", payload)
        return

    template = _EMAIL_TEMPLATES.get(event_type)
    if template is None:
        logging.warning("No email template for eventType=%s — skipping.", event_type)
        return

    subject, html_body = template
    _send_email(recipient, subject, html_body)
