import { Component, ElementRef, ViewChild } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { AuthService } from 'src/app/services/auth.service';

interface Message {
  sender: 'user' | 'bot';
  text: string;
  time: Date;
}

@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent {
  @ViewChild('chatBody') chatBody!: ElementRef;

  isOpen = false;
  isTyping = false;
  userInput = '';

  messages: Message[] = [
    {
      sender: 'bot',
      text: 'Hi! I am your Cooking Hub assistant 🍳 Ask me about classes, get BMI or calorie calculations, or cooking tips!',
      time: new Date()
    }
  ];

  quickReplies = [
    'Suggest beginner classes',
    'Classes under $50',
    'Calculate my BMI',
    'Italian cuisine classes',
    'Calculate my calories'
  ];

  constructor(
    private classService: CookingClassService,
    private auth: AuthService
  ) {}

  toggle() {
    this.isOpen = !this.isOpen;
  }

  useQuickReply(text: string) {
    this.userInput = text;
    this.sendMessage();
  }

  sendMessage() {
    const text = this.userInput.trim();
    if (!text || this.isTyping) return;

    this.messages.push({ sender: 'user', text, time: new Date() });
    this.userInput = '';
    this.isTyping = true;
    this.scrollToBottom();

    this.classService.sendChatMessage(text).subscribe({
      next: (res) => {
        this.isTyping = false;
        this.messages.push({
          sender: 'bot',
          text: res.reply,
          time: new Date()
        });
        this.scrollToBottom();
      },
      error: () => {
        this.isTyping = false;
        
        this.messages.push({
          sender: 'bot',
          text: 'Sorry, something went wrong. Please try again.',
          time: new Date()
        });
        this.scrollToBottom();
      }
    });
  }

  onEnter(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  private scrollToBottom() {
    setTimeout(() => {
      if (this.chatBody?.nativeElement) {
        this.chatBody.nativeElement.scrollTop =
          this.chatBody.nativeElement.scrollHeight;
      }
    }, 100);
  }
}