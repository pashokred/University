import os
from google.auth.transport.requests import Request
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from google.oauth2.credentials import Credentials
from telegram.ext import CommandHandler, ApplicationBuilder, MessageHandler, filters

# Define the scopes for the Gmail API
SCOPES = ['https://www.googleapis.com/auth/gmail.readonly']

# Authenticate with the Gmail API using the credentials from the JSON file
def authenticate_gmail():
    creds = None
    # The file token.json stores the user's access and refresh tokens, and is
    # created automatically when the authorization flow completes for the first
    # time.
    if os.path.exists('token.json'):
        creds = Credentials.from_authorized_user_file('token.json', SCOPES)
    # If there are no (valid) credentials available, let the user log in.
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file(
                'credentials.json', SCOPES)
            creds = flow.run_local_server(port=0)
        # Save the credentials for the next run
        with open('token.json', 'w') as token:
            token.write(creds.to_json())
    service = build('gmail', 'v1', credentials=creds)
    return service

# Get the last 5 messages from the user's Gmail inbox
def get_last_5_messages():
    service = authenticate_gmail()
    results = service.users().messages().list(userId='me', maxResults=5).execute()
    messages = results.get('messages', [])
    return messages

# Handler for the /start command in Telegram
async def start(update, context):
    await update.message.reply_text('Please send me your Gmail API credentials in a file. To obtain the credentials file, follow these steps:\n\n'
                              '1. Go to the Google Cloud Console at https://console.cloud.google.com/\n'
                              '2. Create a new project or select an existing one.\n'
                              '3. Enable the Gmail API for your project.\n'
                              '4. Create a new set of credentials by selecting "Service account key" as the type of credentials.\n'
                              '5. Choose "JSON" as the key type and click "Create".\n'
                              '6. Download the JSON file containing the credentials and send it to me.\n\n'
                                    'To view messages, send file and then run /messages\n'
                                    'To delete data use command /stop\n\n'
                                    'Also, you can do this steps:\n'
                                    'In the Google Cloud console, go to Menu menu > APIs & Services > Credentials.\n'
                                    "Click Create Credentials > OAuth client ID."
                                    'Click Application type > Desktop app.\n'
                                    "In the Name field, type a name for the credential. This name is only shown in the Google Cloud console.\n"
                                    "Click Create. The Auth client created screen appears, showing your new Client ID and Client secret.\n"
                                    "Click OK. The newly created credential appears under Auth 2.0 Client IDs.\n"
                                    "Save the downloaded JSON file as credentials. json, and move the file to bot.")


# Handler for receiving the file with credentials from the user
async def receive_credentials(update, context):
    # Save the received file
    file = await context.bot.get_file(update.message.document.file_id)
    file_path = os.path.join(os.getcwd(), 'credentials.json')
    await file.download_to_drive(file_path)
    await update.message.reply_text('Credentials added')

async def getMessages(update, context):
    # Retrieve the last 5 messages from the user's Gmail inbox
    if not os.path.exists("credentials.json"):
        await update.message.reply_text('No credentials. Add you credentials file')
        return

    messages = get_last_5_messages()
    # Send the messages to the user on Telegram
    if not messages:
        await update.message.reply_text('No messages found.')
    else:
        service = authenticate_gmail()
        for message in messages:
            msg = service.users().messages().get(userId='me', id=message['id']).execute()
            body = msg['payload']['body']
            text = ''
            await update.message.reply_text(msg['snippet'])

async def leave(update, context):
    if os.path.exists("credentials.json"):
        os.remove("credentials.json")
    if os.path.exists('token.json'):
        os.remove('token.json')

    await update.message.reply_text('all your data deleted')


# Initialize the Telegram bot and add the command handlers
bot_token = 'BOT_TOKEN'

application = ApplicationBuilder().token(bot_token).build()
application.add_handler(CommandHandler('start', start))
application.add_handler(MessageHandler(filters.Document.MimeType('application/json'), receive_credentials))
application.add_handler(CommandHandler('messages', getMessages))
application.add_handler(CommandHandler('stop', leave))

# Start the Telegram bot
application.run_polling()
