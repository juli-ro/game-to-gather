import {Component, signal, Signal} from '@angular/core';
import {GroupService} from '../../../shared/group.service';
import {IGroup} from '../../../models/group';
import {ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';
import {FeedbackService} from '../../../shared/feedback.service';
import {IGame} from '../../../models/game';
import {GameService} from '../../games/game.service';
import {MatExpansionModule} from '@angular/material/expansion';

@Component({
  selector: 'app-group-detail',
  imports: [
    MatExpansionModule
  ],
  templateUrl: './group-detail.component.html',
  styleUrl: './group-detail.component.scss',
  standalone: true
})
export class GroupDetailComponent {

  group: Signal<IGroup | null>
  groupGameList: Signal<IGame[]>
  readonly panelOpenState = signal(true);

  constructor(private groupService: GroupService,
              private gameService: GameService,
              private feedbackService: FeedbackService,
              private route: ActivatedRoute,
              private location: Location) {
    this.group = this.groupService.publicSignalItem
    this.groupGameList = this.gameService.publicSignalGroupGameList
  }

  async ngOnInit() {
    try {
      const id = String(this.route.snapshot.paramMap.get("id"));
      if (id) {
        await this.groupService.getItemById(id)
      }
      const group = this.group()
      if(group){
        await this.gameService.getGroupGameList(group.id)
      }

    } catch (e) {
      this.feedbackService.openSnackBarTimed("meeting could not be found", "Close", 4000)
      this.location.back()
    }
  }


}
