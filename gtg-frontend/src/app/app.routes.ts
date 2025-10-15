import { Routes } from '@angular/router';
import {GameListUserComponent} from './components/games/game-list-user/game-list-user.component';
import {LoginComponent} from './components/login/login.component';
import {GameListGroupComponent} from './components/games/game-list-group/game-list-group.component';
import {UserDashboardComponent} from './components/user-dashboard/user-dashboard.component';
import {MeetDetailComponent} from './components/meet/meet-detail/meet-detail.component';
import {GameDetailComponent} from './components/games/game-detail/game-detail.component';
import {GroupListComponent} from './components/groups/group-list/group-list.component';
import {GroupDetailComponent} from './components/groups/group-detail/group-detail.component';
import {UserListComponent} from './components/users/user-list/user-list.component';
import {UserDetailComponent} from './components/users/user-detail/user-detail.component';
import {UserSettingsComponent} from './components/users/user-settings/user-settings.component';

export const routes: Routes = [
  {path: "", component: LoginComponent},
  {path: "game", component: GameListUserComponent},
  {path: "groupGames", component: GameListGroupComponent},
  {path: "login", component: LoginComponent},
  {path: "user-dashboard", component: UserDashboardComponent},
  {path: "meet-detail/:id", component: MeetDetailComponent},
  {path: "game-detail/:id", component: GameDetailComponent},
  {path: "group", component: GroupListComponent},
  {path: "group-detail/:id", component: GroupDetailComponent},
  {path: "user-list", component: UserListComponent},
  {path: "user-detail/:id", component: UserDetailComponent},
  {path: "user-settings", component: UserSettingsComponent}

];
