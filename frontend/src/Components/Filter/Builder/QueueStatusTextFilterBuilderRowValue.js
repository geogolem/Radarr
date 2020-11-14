import React from 'react';
import translate from 'Utilities/String/translate';
import FilterBuilderRowValue from './FilterBuilderRowValue';

const protocols = [
  { id: 'downloading', name: translate('Downloading') },
  { id: 'error', name: translate('Error') },
  { id: 'paused', name: translate('Paused') }
];

function QueueStatusTextFilterBuilderRowValue(props) {
  return (
    <FilterBuilderRowValue
      tagList={protocols}
      {...props}
    />
  );
}

export default QueueStatusTextFilterBuilderRowValue;
