import json
import logging
import azure.functions as func

app = func.FunctionApp()


@app.service_bus_queue_trigger(
    arg_name="message",
    queue_name="stafffinance-notifications",
    connection="SERVICEBUS_CONNECTION")
def notification_dispatcher(message: func.ServiceBusMessage) -> None:
    payload = json.loads(message.get_body().decode("utf-8"))
    subject = getattr(message, "subject", "unknown")

    logging.info("Received notification event. subject=%s payload=%s", subject, payload)
    logging.info(
        "Use Azure Communication Services Email or SendGrid for email delivery. "
        "Use Notification Hubs for device/browser push notifications.")
