
# TenBis Aggregation Bot

1. Aggregation: The bot collects funds over a specific time period, which you can customize using the TaskScheduler.
2. Notification: After the aggregation process, the bot notifies you whether it succeeded or encountered any issues.
3. Telegram Integration: For added convenience, you can configure the bot to interact with you via Telegram. It will prompt you to decide whether to run the aggregation script and provide notifications directly through the Telegram platform.

## Authors

- [@TimaKuDev](https://www.github.com/TimaKuDev)


## FAQ

### Could I change the amount to aggregate?
As of now, the script aggregates the maximum amount for the current day. Unfortunately, there isn’t an option to customize the aggregation amount.

### Would the script run if I don’t select an answer in Telegram?
No, the script won’t run by default if you don’t select an answer in Telegram. It only executes when you explicitly choose to run it through the Telegram interface.

### Is my PC required to be on?
Yes, the script runs on your PC using Selenium. Make sure your computer is powered on and connected to the internet for the script to function properly.

### What browsers can I use?
The script currently works in the following browsers:
- Chrome (Version: 122.0.6261.9400)
- Edge (Version: 120.0.2210.144)
- Firefox (Version: 0.27.0)
## Features

1. Progression Notifications:
- Keep your contacts informed about the project’s progress. Whether it’s milestones achieved, updates, or critical events, timely notifications ensure transparency and effective communication.
2.  Cross-Browser Compatibility:
- Run the script seamlessly on three popular browsers:
    - Chrome (Version: 122.0.6261.9400)
    - Edge (Version: 120.0.2210.144)
    - Firefox (Version: 0.27.0)
- This flexibility allows users to choose their preferred browser without compromising functionality.
3. Notification Channels:
- Choose how you want to receive notifications:
    - Telegram: Receive updates directly through Telegram.
    - Email: Stay informed via email notifications.

## Installation

Follow these steps to set up and configure your project:

1. [Create the Executable (EXE) File:](https://www.youtube.com/watch?v=7iVIfkVHKII&t=2s&ab_channel=DarrenDoesEverything)
- Compile your script into an executable (EXE) file. This step ensures that your application can be easily installed and run on various systems.

2. Install the Application:
- After creating the EXE file, install the application on your computer. This step typically involves running the installer and following the installation prompts.

3. Configure Browser Settings:
 - Open the BrowserSettings file to specify which browser you want the script to run on. For example:
**JSON**
```
{
    "BrowserType": "Chrome",
    "UserProfilePath": "C:\\Users\\User\\AppData\\Local\\Google\\Chrome\\User Data\\Default"
}
```
Customize the browser type and user profile path according to your preferences.

4. Choose Notification Method:
- Decide how you want to receive notifications:
    * Telegram:
        * Change the NotifyType to “Telegram”.
        * Go to Telegram and find “BotFather” Run the command ```/newbot``` and follow the instructions to receive an access token.
        * Find “Telegram Bot Raw” and run ```/start``` to get a JSON response. Copy the message.chat.id.
        * Insert both the token and chat ID into the CommunicationSettings:
        **JSON**
        ```
        {
            "NotifyType": "Telegram",
            "Token": "1:1-eBc",
            "ChatId": "1"
        }
        ```

    * Email:
        * Change the NotifyType to “Email” and provide your email address in the NotifyTo field:
        **JSON**
        ```
        {
            "NotifyType": "Email",
            "NotifyTo": "Name@gmail.com"
        }
        ```

5. Task Scheduler Setup:
* Open Task Scheduler on your computer.
* Create a basic task:
    * Set the time and date for the script to run.
    * Edit the action and add the script’s path (optional).
* Run the script once to ensure that the contact is logged in.

## Lessons Learned
1. Project Design and Extensibility:
* I’ve gained insights into the importance of thoughtful project design. Considering extensibility from the start allows for smoother future enhancements and scalability.

2. Working with Telegram.Bot:
* Learning how to integrate Telegram.Bot was a valuable experience. It opened up communication channels and allowed me to interact with users effectively.

3. Selenium Usage:
* Exploring Selenium for web automation was enlightening. It’s a powerful tool for interacting with web pages programmatically.

4. Code Organization:
* Organizing my code effectively improved readability and maintainability. Clear structure and naming conventions make collaboration and debugging easier.

5. Design Patterns Implementation:
* Implementing design patterns, such as Singleton or Factory, enhanced code quality and made it more robust.
## Roadmap

1. Selenium Bot Creation:
* Successfully developed the Selenium Bot, which serves as the foundation for data aggregation and interaction.

2. User Notification Implementation:
* Added functionality to notify users about the Bot’s existence and purpose. Clear communication ensures user engagement.

3. Telegram Integration:
* Integrated Telegram as a notification channel. Users can now interact with the Bot through Telegram.

4. Future Enhancement:
* In the future, consider transitioning from Selenium to a more efficient Ten Bis API call for data retrieval.


## Screenshots

<img width="463" alt="image" src="https://github.com/TimaKuDev/TenBis/assets/53705199/19bb5500-abdb-4c87-b34d-b404eb6827f7">



## Support

For support, email TimaKuDev@gmail.com.


## Tech Stack

**Server:** C#

**Bot:** Selenium

**Communication:** Telegram
