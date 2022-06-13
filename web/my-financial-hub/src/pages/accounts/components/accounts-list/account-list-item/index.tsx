import { Account } from '../../../../../commom/interfaces/account';

import { useAccountsContext } from '../../../contexts/accounts-page-context';
import { useApisContext } from '../../../../../commom/contexts/api-context';

import style from './account-list__item.module.scss';

interface AccountListItemProps {
  account: Account
}

export default function AccountListItem({ account }: AccountListItemProps) {
  const [state, setState] = useAccountsContext();
  const {accountsApi} = useApisContext();

  const deleteAccount = function (id?: string) {
    if(id){
      accountsApi.DeleteAsync(id)
        .then(() => setState(
          {
            account: { name: '', description: '', currency: '', isActive: false } as Account,
            accounts: state.accounts.filter(x => x.id !== id)
          }
        ))
        .catch(e => console.error(e));
    }
  };

  const selectAccount = function () {
    setState({ ...state, account: account });
  };

  return (
    //${account.isActive?style.inactive:style.inactive}
    <div className={`row d-flex p-2 align-middle ${style.row}`}>
      <div className='col-11 d-flex' onClick={() => selectAccount()}>
        <div className='col-3'>
          {account.name}
        </div>
        <div className='col-6'>
          {account.description}
        </div>
        <div className='col-1'>
          {account.currency}
        </div>
        <div className='col-1'>
          {account.isActive ? 'yes' : 'no'}
        </div>
      </div>
      <div className='col-1'>
        <button onClick={() => deleteAccount(account.id)}>Delete</button>
      </div>
    </div>
  );
} 