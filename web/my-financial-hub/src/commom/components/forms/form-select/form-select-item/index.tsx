import SelectOption from '../types/select-option';

export default function FormSelectItem(
  {
    option, isSelected,
    onSelect,onDelete
  }:
  {
    option: SelectOption, isSelected: boolean,
    onSelect: (option: SelectOption) => void,onDelete: (id:string) => void
  }
) {

  return (
    <li
      role="option"
      aria-selected={isSelected}
    >
      <ul>
        <li 
          data-testid={'select-' + option.value}  aria-label={option.label} 
          onClick={() => onSelect(option)}
        >
          {option.label}
        </li>
        <li 
          data-testid={'delete-' + option.value}  aria-label={'delete ' + option.label} 
          onClick={() => onDelete(option.value)}
        >
          delete
        </li>
      </ul>
    </li>
  );
}